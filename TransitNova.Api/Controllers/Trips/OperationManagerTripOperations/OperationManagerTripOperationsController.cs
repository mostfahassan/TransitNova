using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Trips.OperationManagerTripOperations
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-manager/trips")]
    [Tags("Operation Manager Trips")]
    public class OperationManagerTripOperationsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.StartPickupTrip)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{carrierId:guid}/start-pickup")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Start Pickup Trip")]
        [EndpointSummary("Start a pickup trip")]
        [EndpointDescription("Allows an authorized operation manager to start a pickup trip. The trip status is transitioned to the active pickup stage and becomes available for shipment collection operations.")]
        public async Task<IActionResult> StartPickupTrip([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid carrierId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new StartPickUpTripCommand(parsedRequestId, operationManagerId, carrierId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.StartDeliveryTrip)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPatch("{carrierId:guid}/start-delivery")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Start Delivery Trip")]
        [EndpointSummary("Start a delivery trip")]
        [EndpointDescription("Allows an authorized operation manager to start a delivery trip. The trip status is transitioned to the active delivery stage and the assigned shipments become ready for delivery operations")]
        public async Task<IActionResult> StartDeliveryTrip([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid carrierId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new StartDeliveryTripCommand(parsedRequestId, operationManagerId, carrierId), ct);
            return response.ToActionResult();
        }
    }
}
