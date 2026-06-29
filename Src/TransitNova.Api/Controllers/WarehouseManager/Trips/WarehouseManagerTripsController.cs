using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Trips;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.WarehouseManager.Trips
{
    [Authorize(Roles = Role.WarehouseManager)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/warehouse-managers/trips")]
    [Tags("Warehouse Manager Trips")]
    public sealed class WarehouseManagerTripsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = WarehouseManagerPermissions.ViewTrips)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Trips")]
        [EndpointSummary("Get trips for the authenticated warehouse manager")]
        [EndpointDescription("Returns trips associated with the authenticated warehouse manager's warehouse.")]
        public async Task<IActionResult> TripsAsync(Guid warehouseId, [FromQuery] FilterTripsDto filter,CancellationToken ct)
        {
            if (!await IsWrehouseManagerAsync(warehouseId)) return Forbid();
            filter.WarehouseId = warehouseId;
            var response = await mediator.Send(new FilterTripsQuery(filter), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = WarehouseManagerPermissions.ViewTripDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{tripId:guid}/warehouse/{warehouseId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Trip")]
        [EndpointSummary("Get trip for the authenticated warehouse manager")]
        [EndpointDescription("Returns trip details when the trip belongs to the authenticated warehouse manager's warehouse.")]
        public async Task<IActionResult> TripAsync(Guid warehouseId, Guid tripId,CancellationToken ct)
        {
            if (!await IsWrehouseManagerAsync(warehouseId)) return Forbid();
            var response = await mediator.Send(new GetTripDetailsQuery(tripId), ct);
            return response.ToActionResult();
        }



        private async Task<bool> IsWrehouseManagerAsync(Guid warehouseId)
        {
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, warehouseId, WarehouseManagerPermissions.IsWarehouseManager);
            return authorizationResult.Succeeded;
        }
    }
}

