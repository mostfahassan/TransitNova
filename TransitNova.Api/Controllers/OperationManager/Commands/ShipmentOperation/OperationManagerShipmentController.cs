
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.OperationManager.Commands.ShipmentOperation
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [Route("api/v{version:apiVersion}/operation-manager/shipments")]
    [ApiVersion("1.0")]
    [Tags("Operation Manager Shipment Commands")]
    [ApiController]
    public class OperationManagerShipmentController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.ApproveShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{shipmentId:guid}/approve")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Approve Shipment")]
        [EndpointSummary("Approve a pending shipment")]
        [EndpointDescription("Allows an authorized operation manager to approve a pending shipment.Once approved, the shipment status is updated and becomes eligible for the next stage of the shipping workflow. " +
                 "The operation requires the appropriate shipment approval permission.")]
        public async Task<IActionResult> ApproveShipment([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid shipmentId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new ApproveShipmentCommand(parsedRequestId, operationManagerId, shipmentId),ct);
            return response.ToActionResult();
        }


        [Authorize(Policy = OperationManagerPermissions.RejectShipment)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{shipmentId:guid}/reject")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Reject Shipment")]
        [EndpointSummary("Reject a pending shipment")]
        [EndpointDescription("Allows an authorized operation manager to reject a pending shipment and provide a rejection reason.The shipment status is updated to rejected, and the supplied reason is recorded for auditing and customer communication purposes."
              + "The operation requires the appropriate shipment rejection permission.")]  
        public async Task<IActionResult> RejectShipment([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid shipmentId, [FromBody] RejectShipmentReason request, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new RejectShipmentCommand(parsedRequestId, operationManagerId, shipmentId, request.RejectionReason),ct);
            return response.ToActionResult();
        }
    }
}
