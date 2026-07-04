using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Trips;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Trips.CarriersOperation
{
    [Authorize(Roles = Role.Carrier)]
    [Authorize(Policy = CarrierPermissions.HasCompletedProfile)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/carriers")]
    [Tags("Operation Manager Trips")]
    public class CarrierTripsController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/trips")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Trips")]
        [EndpointSummary("Get trips for the authenticated carrier")]
        [EndpointDescription("Returns the trips assigned to the authenticated carrier.")]
        public async Task<IActionResult> TripsAsync(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();
            var result = await mediator.Send(new GetCarrierTripsQuery(carrierId), ct);
            return result.ToActionResult();
        }


        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/trips/{tripId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Trip ById")]
        [EndpointSummary("Get details of a specific carrier trip")]
        [EndpointDescription("Returns the detailed information for a trip that belongs to the authenticated carrier.")]
        public async Task<IActionResult> TripAsync(Guid tripId, Guid carrierId,  CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();
            var result = await mediator.Send(new GetCarrierTripDetailsQuery(carrierId, tripId), ct);
            return result.ToActionResult();
        }
        private async Task<bool> IsCarrierOwnerAsync(Guid carrierId)
        {
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, carrierId ,CarrierPermissions.IsCarrierOwner);
            return authorizationResult.Succeeded;
        }
    }
}
