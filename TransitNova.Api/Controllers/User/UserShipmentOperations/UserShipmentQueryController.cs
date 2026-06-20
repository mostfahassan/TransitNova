
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
    [Route("api/v{version:apiVersion}/user/shipments")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("User Shipments Query Operations")]
    public class UserShipmentQueryController(IMediator mediator ,  IAuthorizationService authorization) : ControllerBase
    {
        // GET api/v1/user/shipments/{shipmentId}
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
        [EndpointName("Get Shipment Details")]
        [EndpointSummary("Retrieve an existing shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to Show an existing shipment by providing the shipment ID in the request URL. The user must have the 'User.UserCanViewShipmentDetails' permission to access this endpoint.")]
        public async Task<IActionResult> GetUserShipmentDetails(Guid shipmentId, CancellationToken ct)
        {
            if (!await IsUserOwnsShipment(shipmentId))
                return Forbid();
            var userId = User.GetUserId();
            var response = await mediator.Send(new GetUserShipmentQuery(userId, shipmentId), ct);
            return response.ToActionResult();
        }
        private async Task<bool> IsUserOwnsShipment(Guid shipmentId)
        {
            var authorizationResult =
               await authorization.AuthorizeAsync(User, shipmentId, UserPermissions.ShipmentOwner);
            return authorizationResult.Succeeded;
        }
    }

}
