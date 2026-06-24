using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.Api.Controllers.Carrier.Operations
{
    [Authorize(Roles = Role.Carrier)]
    [Authorize(Policy = CarrierPermissions.HasCompletedProfile)]
    [Route("api/v{version:apiVersion}/carriers")]
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Carrier Operations")]
    public class CarrierOperationsController(IMediator mediator,IAuthorizationService authorizationService) : ControllerBase
    {
        [Authorize(Policy = CarrierPermissions.CanUpdateStatus)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{carrierId:guid}/status")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCarrierStatusAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid carrierId, [FromBody] ChangeCarrierStatus dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var response = await mediator.Send(new UpdateCarrierStatusCommand(parsedRequestId, carrierId, dto.Status), ct);
            return response.ToActionResult();
        }


        [Authorize(Policy = CarrierPermissions.CanCompleteDeliveryShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{carrierId:guid}/shipments/{shipmentId:guid}/complete-delivery")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Complete Shipment Delivery")]
        [EndpointSummary("Mark a shipment as delivered")]
        [EndpointDescription("Allows the authenticated carrier to complete the delivery process for an assigned shipment.The shipment status is updated to Delivered after validating carrier ownership and shipment state.")]
        public async Task<IActionResult> CompleteShipmentAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid carrierId,Guid shipmentId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var response = await mediator.Send(new CompleteShipmentCommand(parsedRequestId, shipmentId, carrierId), ct);
            return response.ToActionResult();
        }


        [Authorize(Policy = CarrierPermissions.CanCompletePickupShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{carrierId:guid}/shipments/{shipmentId:guid}/complete-pickup")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Complete Shipment Pickup")]
        [EndpointSummary("Mark a shipment as picked up")]
        [EndpointDescription("Allows the authenticated carrier to complete the pickup process for an assigned shipment.The shipment status is updated to indicate successful pickup and transfer to the warehouse workflow.")]
        public async Task<IActionResult> CompletePickupShipmentAsync([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid carrierId, Guid shipmentId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();
            var response = await mediator.Send(new CompleteShipmentToWarehouseCommand(parsedRequestId, shipmentId, carrierId), ct);
            return response.ToActionResult();
        }
        private async Task<bool> IsCarrierOwnerAsync(Guid carrierId)
        {
            var authorizationResult =
                await authorizationService.AuthorizeAsync(
                    User,
                    carrierId,
                    CarrierPermissions.IsCarrierOwner);

            return authorizationResult.Succeeded;
        }
    }
}
