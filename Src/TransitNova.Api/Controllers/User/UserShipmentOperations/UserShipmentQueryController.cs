
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.User.UserShipmentOperations
{
    [Authorize(Roles = Role.User)]
    [Route("api/v{version:apiVersion}/users/shipments")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("User Shipments Query Operations")]
    public class UserShipmentQueryController(IMediator mediator) : ControllerBase
    {
        // GET api/v1/users/shipments/{shipmentId}
        [Authorize(Policy = UserPermissions.UserCanViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{shipmentId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Shipment ById")]
        [EndpointSummary("Retrieve an existing shipment for the authenticated user.")]
        [EndpointDescription("Allows an authenticated user to retrieve an existing shipment by its identifier. The user must have the 'User.UserCanViewShipmentDetails' permission.")]
        public async Task<IActionResult> GetUserShipmentDetailsAsync(Guid shipmentId, CancellationToken ct)
        {
            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();
            var userId = User.GetUserId();
            var response = await mediator.Send(new GetUserShipmentQuery(userId, shipmentId), ct);
            return response.ToActionResult();
        }
        private async Task<bool> UserOwnsShipmentAsync(Guid shipmentId)
        {
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, shipmentId, UserPermissions.ShipmentOwner);
            return authorizationResult.Succeeded;
        }
    }

}
