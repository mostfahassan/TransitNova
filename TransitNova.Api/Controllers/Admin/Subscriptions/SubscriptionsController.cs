using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Admin.Subscriptions
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/subscriptions")]
    [Tags("Admin Bundle Subscriptions")]
    public sealed class SubscriptionsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewBundleDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{subscriptionId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Subscription Details")]
        [EndpointSummary("Get bundle subscription details")]
        [EndpointDescription("Returns details for a specific bundle subscription.")]
        public async Task<IActionResult> Subscription(Guid subscriptionId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetBundleSubscriptionDetailsQuery(subscriptionId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewBundleDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("bundles/{bundleId:int}/subscribers")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Bundle Subscribers")]
        [EndpointSummary("Get bundle subscribers")]
        [EndpointDescription("Returns all active users subscribed to a specific bundle.")]
        public async Task<IActionResult> BundleSubscribers(int bundleId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetBundleSubscribersQuery(bundleId), ct);
            return response.ToActionResult();
        }
    }
}
