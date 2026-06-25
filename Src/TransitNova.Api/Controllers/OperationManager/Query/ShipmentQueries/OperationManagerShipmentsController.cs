using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.OperationManager.Query.ShipmentQueries
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-managers/shipments")]
    [Tags("Operation Manager Shipments")]
    public sealed class OperationManagerShipmentsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.ViewAssignedShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("assigned")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Assigned Shipments")]
        [EndpointSummary("Get shipments assigned to the operation manager")]
        [EndpointDescription("Returns the list of shipments assigned to the authenticated operation manager using the provided filter criteria.")]
        public async Task<IActionResult> AssignedAsync([FromQuery] ShipmentFilterDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new GetAssignedShipmentsQuery(filter), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{shipmentId:guid}/review")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Review Shipment")]
        [EndpointSummary("Get a shipment for review")]
        [EndpointDescription("Returns the shipment details required by the operation manager to review a shipment.")]
        public async Task<IActionResult> ReviewShipmentAsync(Guid shipmentId, CancellationToken ct)
        {
            var operationManagerId = User.GetUserId();
            var response = await mediator.Send(new ReviewShipmentCommand(shipmentId, operationManagerId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewShipmentHistory)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{shipmentId:guid}/histories")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Shipment Histories")]
        [EndpointSummary("Get all shipment status histories")]
        [EndpointDescription("Returns the full status history timeline for the specified shipment.")]
        public async Task<IActionResult> HistoriesAsync(Guid shipmentId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetShipmentHistoriesQuery(shipmentId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewAllShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Shipments")]
        [EndpointSummary("Get filtered shipments for the operation manager")]
        [EndpointDescription(
          "Returns the filtered shipments list available to the authenticated operation manager.")]
        public async Task<IActionResult> ShipmentsAsync([FromQuery] ShipmentFilterDto filter, CancellationToken ct)
        {
            var result = await mediator.Send(new FilterShipmentsQuery(filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewPendingShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("review-queue")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Review Queue")]
        [EndpointSummary("Get shipments waiting for review")]
        [EndpointDescription("Returns the shipments that are currently waiting in the review queue for the authenticated operation manager.")]

        public async Task<IActionResult> ReviewQueueAsync([FromQuery] ShipmentFilterDto filter, CancellationToken ct)
        {
            var result = await mediator.Send(new FilterShipmentsQuery(filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{shipmentId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Shipment")]
        [EndpointSummary("Get shipment details by id")]
        [EndpointDescription("Returns the full shipment details for the specified shipment identifier.")]

        public async Task<IActionResult> ShipmentAsync(Guid shipmentId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetShipmentByIdQuery(shipmentId), ct);
            return result.ToActionResult();
        }
    }
}
