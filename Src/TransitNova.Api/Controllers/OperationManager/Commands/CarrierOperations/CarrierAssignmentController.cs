using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.OperationManager.Commands.CarrierOperations
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [Route("api/v{version:apiVersion}/operation-managers/carriers")]
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Operation Manager Carrier Assignment Operation")]
    public class CarrierAssignmentController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.AssignPickupCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{shipmentId:guid}/assign-pickup")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Assign Pickup Carrier")]
        [EndpointSummary("Assign a pickup carrier to a shipment")]
        [EndpointDescription("Allows an authorized operation manager to assign a pickup carrier to an approved shipment.The assigned carrier becomes responsible for collecting the shipment from the sender and initiating the pickup process.")]
        public async Task<IActionResult> AssignPickupCarrierAsync([IdempotencyKey] Guid requestId, Guid shipmentId, [FromBody] AssignCarrierDto request, CancellationToken ct)
        {

            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new AssignShipmentPickupToCarrierCommand(requestId, shipmentId, operationManagerId, request.CarrierId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.AssignDeliveryCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{shipmentId:guid}/assign-delivery")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Assign Delivery Carrier")]
        [EndpointSummary("Assign a delivery carrier to a shipment")]
        [EndpointDescription("Allows an authorized operation manager to assign a delivery carrier to a shipment.The assigned carrier becomes responsible for transporting the shipment from the warehouse to the final recipient.")]
        public async Task<IActionResult> AssignDeliveryCarrierAsync([IdempotencyKey] Guid requestId, Guid shipmentId, [FromBody] AssignCarrierDto request, CancellationToken ct)
        {

            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new AssignShipmentDeliveryToCarrierCommand(requestId, shipmentId, operationManagerId, request.CarrierId), ct);
            return response.ToActionResult();
        }
    }
}
