using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Trips;
using TransitNova.BusinessLayer.Features.Trips.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Admin.Trips
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/trips")]
    [Tags("Admin Trips")]
    public sealed class AdminTripsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewTrips)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Trips")]
        [EndpointSummary("Get trips for admin users")]
        [EndpointDescription("Returns paginated trips for admin users using the provided filter criteria.")]
        public async Task<IActionResult> TripsAsync([FromQuery] FilterTripsDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new FilterTripsQuery(filter), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewTripDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{tripId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Trip")]
        [EndpointSummary("Get trip details for admin users")]
        [EndpointDescription("Returns full trip details for a specific trip identifier.")]
        public async Task<IActionResult> TripAsync(Guid tripId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetTripDetailsQuery(tripId), ct);
            return response.ToActionResult();
        }
    }
}