using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Trips;
using TransitNova.BusinessLayer.Features.Trips.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.OperationManager.Query.Trips
{
    [Authorize(Roles = Role.OperationManager)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-managers/trips")]
    [Tags("Operation Manager Trips")]
    public sealed class OperationManagerTripsController(IMediator mediator, IOperationManagerQueryRepository operationManagerQueryRepository) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.ViewTrips)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Trips")]
        [EndpointSummary("Get trips for the authenticated operation manager")]
        [EndpointDescription("Returns paginated trips scoped to carriers handled by the authenticated operation manager.")]
        public async Task<IActionResult> TripsAsync([FromQuery] FilterTripsDto filter, CancellationToken ct)
        {
            var handlerId = await ResolveHandlerIdAsync(ct);
            if (!handlerId.HasValue)
                return Forbid();

            filter.HandlerId = handlerId.Value;
            var response = await mediator.Send(new FilterTripsQuery(filter), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = OperationManagerPermissions.ViewTripDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{tripId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Trip")]
        [EndpointSummary("Get trip details for the authenticated operation manager")]
        [EndpointDescription("Returns trip details only when the trip belongs to a carrier handled by the authenticated operation manager.")]
        public async Task<IActionResult> TripAsync(Guid tripId, CancellationToken ct)
        {
            var handlerId = await ResolveHandlerIdAsync(ct);
            if (!handlerId.HasValue)
                return Forbid();

            var response = await mediator.Send(new GetTripDetailsQuery(tripId, handlerId.Value), ct);
            return response.ToActionResult();
        }

        private async Task<Guid?> ResolveHandlerIdAsync(CancellationToken cancellationToken)
        {
            var appUserId = User.GetUserId();
            if (appUserId == Guid.Empty)
                return null;

            var operationManagerId = await operationManagerQueryRepository.GetUserIdAsync(appUserId, cancellationToken);
            return operationManagerId == Guid.Empty ? null : operationManagerId;
        }
    }
}