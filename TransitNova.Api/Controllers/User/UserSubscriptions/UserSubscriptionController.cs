using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.User.UserSubscriptions
{
    [Authorize(Roles = Role.User)]
    [Authorize(Policy = UserPermissions.ShipmentOwner)]
    [Route("api/v{version:apiVersion}/subscriptions")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("User Subscription Controller")]
    public class UserSubscriptionController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = UserPermissions.UserCanSubscribeBundle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("bundles/{bundleId:guid}/subscription")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Subscribe To Bundle")]
        [EndpointSummary("Subscribe the authenticated user to a bundle")]
        [EndpointDescription("Allows the authenticated user to subscribe to a bundle. The operation validates bundle availability, subscription eligibility, and applies the selected bundle to the user's account.")]
        public async Task<IActionResult> SubscribeToBundle([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid bundleId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var userId = User.GetUserId();
            var response = await mediator.Send(new SubscribeToBundleCommand(parsedRequestId, userId, bundleId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = UserPermissions.UserCanUnSubscribeBundle)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("bundles/{bundleId:guid}/subscription")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Unsubscribe From Bundle")]
        [EndpointSummary("Cancel the user's bundle subscription")]
        [EndpointDescription("Allows the authenticated user to cancel an active bundle subscription. The operation validates the current subscription status and removes the bundle association from the user's account.")]
        public async Task<IActionResult> UnSubscribeFromBundle([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid bundleId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var userId = User.GetUserId();
            var response = await mediator.Send(new UnSubscribeBundleCommand(parsedRequestId, userId, bundleId), ct);
            return response.ToActionResult();
        }

    }
}
