using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Carrier;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.User.UserCarrierOperation
{
    [Authorize(Roles = Role.User)]
    [Route("api/v{version:apiVersion}/shipments")]
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("User Shipment Ratings")]
    public sealed class ShipmentRatingsController(IMediator mediator,IAuthorizationService authorizationService) : ControllerBase
    {

        [Authorize(Policy = UserPermissions.CanRatePickupCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("{shipmentId:guid}/rate-pickup-carrier")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Rate Pickup Carrier")]
        [EndpointSummary("Rate the carrier who handled shipment pickup")]
        [EndpointDescription(
            "Allows the authenticated shipment owner to rate the carrier responsible for pickup. " +
            "The operation validates that the shipment can be rated, that the carrier exists, and then stores the rating.")]
        public async Task<IActionResult> RatePickupCarrierAsync([IdempotencyKey] Guid requestId, Guid shipmentId, [FromBody] RatingCarrierDto dto, CancellationToken ct)
        {

            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();

            var userId = User.GetUserId();
            var response = await mediator.Send(new RatePickupCarrierCommand(requestId, userId, shipmentId, dto), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = UserPermissions.CanRateDeliveryCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("{shipmentId:guid}/rate-delivery-carrier")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Rate Delivery Carrier")]
        [EndpointSummary("Rate the carrier who handled shipment delivery")]
        [EndpointDescription(
            "Allows the authenticated shipment owner to rate the carrier responsible for delivery. " +
            "The operation validates that the shipment can be rated, that the carrier exists, and then stores the rating.")]
        public async Task<IActionResult> RateDeliveryCarrierAsync([IdempotencyKey] Guid requestId, Guid shipmentId, [FromBody] RatingCarrierDto dto, CancellationToken ct)
        {

            if (!await UserOwnsShipmentAsync(shipmentId))
                return Forbid();

            var userId = User.GetUserId();
            var response = await mediator.Send(new RateDeliveryCarrierCommand(requestId, userId, shipmentId, dto), ct);
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
