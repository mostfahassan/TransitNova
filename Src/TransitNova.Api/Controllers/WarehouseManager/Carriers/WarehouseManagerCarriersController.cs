using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.WarehouseManager.Carriers
{
    [Authorize(Roles = Role.WarehouseManager)]
    [ApiController]
    [Route("api/v{version:apiVersion}/warehouse-managers/carriers")]
    [ApiVersion("1.0")]
    [Tags("Warehouse Manager Carriers")]
    public sealed class WarehouseManagerCarriersController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = WarehouseManagerPermissions.ViewCarriers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Carriers")]
        [EndpointSummary("Get carriers for the authenticated warehouse manager")]
        [EndpointDescription("Returns carriers assigned to the authenticated warehouse manager's warehouse.")]
        public async Task<IActionResult> CarriersAsync(Guid warehouseId , [FromQuery] FilterCarrierDto filter, CancellationToken ct)
        {
            if (!await IsWrehouseManagerAsync(warehouseId)) return Forbid(); 
            filter.WarehouseId = warehouseId;
            var result = await mediator.Send(new GetCarriersForOperationManagerQuery(filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = WarehouseManagerPermissions.ViewCarrierDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/warehouse/{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Carrier")]
        [EndpointSummary("Get carrier for the authenticated warehouse manager")]
        [EndpointDescription("Returns carrier details when the carrier belongs to the authenticated warehouse manager's warehouse.")]
        public async Task<IActionResult> CarrierAsync(Guid warehouseId, Guid carrierId, CancellationToken ct)
        {
            if (!await IsWrehouseManagerAsync(warehouseId)) return Forbid();
            var result = await mediator.Send(new GetCarrierProfileQuery(carrierId), ct);
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

