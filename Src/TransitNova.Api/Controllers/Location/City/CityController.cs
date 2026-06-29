using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Location.City
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cities")]
    [Tags("Cities")]
    public class CityController(IMediator mediator):ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{cityId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get City")]
        [EndpointSummary("Get city details")]
        [EndpointDescription("Returns city details by city identifier.")]
        public async Task<IActionResult> CityAsync(int cityId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetCityByIdQuery(cityId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Cities")]
        [EndpointSummary("Get cities")]
        [EndpointDescription("Returns paginated cities using the provided filter criteria.")]
        public async Task<IActionResult> CitiesAsync([FromQuery] CityFilterDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new FilterCitiesQuery(filter), ct);
            return response.ToActionResult();
        }
    }
}
