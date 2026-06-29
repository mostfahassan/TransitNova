using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.User.UserShipmentOperations
{
    [Authorize(Roles = Role.User)]
    [Route("api/v{version:apiVersion}/users/shipments")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("User Shipments Operations")]
    public class UserShipmentOperationsController(IMediator mediator,IAuthorizationService authorizationService) : ControllerBase
    {     
        
        // PUT api/v1/shipments/{shipmentId}
        [Authorize(Policy = UserPermissions.UserCanUpdateShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{shipmentId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("UpdateShipmentDetails")]
        [EndpointSummary("Updates an existing shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to update an existing shipment by providing the updated shipment details in the request body. The user must have the 'User.UpdateShipmentDetails' permission to access this endpoint.")]
        public async Task<IActionResult> UpdateShipmentAsync([IdempotencyKey] Guid requestId, Guid shipmentId, [FromBody] UpdateShipmentDto dto, CancellationToken ct)
        {

            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();
            var userId = User.GetUserId();
            var response = await mediator.Send(new UpdateShipmentCommand(requestId, userId, shipmentId, dto), ct);
            return response.ToActionResult();
        }

        // PATCH api/v1/shipments/{shipmentId}
        [Authorize(Policy = UserPermissions.UserCanCancelShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{shipmentId:guid}/cancel")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("CancelShipment")]
        [EndpointSummary("Cancels an existing shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to cancel an existing shipment by providing the shipment ID in the request URL. The user must have the 'User.CancelShipment' permission to access this endpoint.")]
        public async Task<IActionResult> CancelShipmentAsync([IdempotencyKey] Guid requestId, Guid shipmentId, CancellationToken ct)
        {

            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();
            var userId = User.GetUserId();
            var response = await mediator.Send(new CancelShipmentCommand(requestId, userId, shipmentId), ct);
            return response.ToActionResult();
        }



        // PATCH api/v1/shipments/{shipmentId}/issue
        [Authorize(Policy = UserPermissions.UserCanIssueShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{shipmentId:guid}/issue")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("IssueShipment")]
        [EndpointSummary("Issues a Delivered shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to issue a new shipment by providing the shipment details in the request body. The user must have the 'User.IssueShipment' permission to access this endpoint.")]
        public async Task<IActionResult> IssueShipmentAsync([IdempotencyKey] Guid requestId, Guid shipmentId, [FromBody] IssueShipmentReason issue, CancellationToken ct)
        {

            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();
            var userId = User.GetUserId();
            var response = await mediator.Send(new IssueShipmentCommand(requestId, userId, shipmentId, issue.IssueReason), ct);
            return response.ToActionResult();
        }



        // GET api/v1/shipments/{trackingNumber}
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{shipmentId:guid}")]
        [Authorize(Policy = UserPermissions.UserCanDeleteShipment)]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteShipment")]
        [EndpointSummary("Deletes an existing shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to delete an existing shipment by providing the shipment ID in the request URL. The user must have the 'User.DeleteShipment' permission to access this endpoint.")]
        public async Task<IActionResult> DeleteShipmentAsync([IdempotencyKey] Guid requestId, Guid shipmentId, CancellationToken ct)
        {

            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();

            var userId = User.GetUserId();
            var response = await mediator.Send(new DeleteShipmentCommand(requestId, shipmentId, userId), ct);
            return response.ToActionResult();
        }


        // DELETE api/v1/shipments/{shipmentId}
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{trackingNumber}")]
        [Authorize(Policy = UserPermissions.UserCanTrackShipment)]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("TrackShipment")]
        [EndpointSummary("Track an existing shipment for the authenticated user.")]
        [EndpointDescription("This endpoint allows an authenticated user to Track an existing shipment by providing the Tracking Number in the request URL. The user must have the 'User.UserCanTrackShipment' permission to access this endpoint.")]
        public async Task<IActionResult> TrackShipmentAsync(string trackingNumber, CancellationToken ct)
        {
            var response = await mediator.Send(new TrackShipmentQuery(trackingNumber), ct);
            return response.ToActionResult();
        }

        private async Task<bool> UserOwnsShipmentAsync(Guid shipmentId)
        {
            var authorizationResult =
               await authorizationService.AuthorizeAsync(
                   User,
                   shipmentId,
                   UserPermissions.ShipmentOwner);

            return authorizationResult.Succeeded;
        }

    }
}
