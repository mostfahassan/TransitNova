using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.WarehouseManager.Shipments
{
    [Authorize(Roles = Role.WarehouseManager)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/warehouse-managers/shipments")]
    [Tags("Warehouse Manager Shipments")]
    public sealed class WarehouseManagerShipmentsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = WarehouseManagerPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Shipments")]
        [EndpointSummary("Get shipments for the authenticated warehouse manager")]
        [EndpointDescription("Returns shipments associated with the authenticated warehouse manager's warehouse.")]
        public async Task<IActionResult> ShipmentsAsync(Guid warehouseId,[FromQuery] ShipmentFilterDto filter, CancellationToken ct)
        {
            if (!await IsWrehouseManagerAsync(warehouseId)) return Forbid();
            filter.WarehouseId = warehouseId;
            var result = await mediator.Send(new FilterShipmentsQuery(filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = WarehouseManagerPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{shipmentId:guid}/warehouse/{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Shipment")]
        [EndpointSummary("Get shipment for the authenticated warehouse manager")]
        [EndpointDescription("Returns shipment details when the shipment belongs to the authenticated warehouse manager's warehouse.")]
        public async Task<IActionResult> ShipmentAsync(Guid warehouseId, Guid shipmentId, CancellationToken ct)
        {
            if (!await IsWrehouseManagerAsync(warehouseId)) return Forbid();
            var result = await mediator.Send(new GetShipmentByIdQuery(shipmentId), ct);
            return result.ToActionResult();
        }




        private async Task<bool> IsWrehouseManagerAsync(Guid warehouseId)
        {
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, warehouseId, WarehouseManagerPermissions.IsWarehouseManager);
            return authorizationResult.Succeeded;
        }
    }
}

