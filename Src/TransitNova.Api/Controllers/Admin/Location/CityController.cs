using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.Admin.Location
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/cities")]
    [Tags("Admin Cities")]
    public sealed class CityController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Create City")]
        [EndpointSummary("Create a new city")]
        [EndpointDescription("Creates a new city under a government.")]
        public async Task<IActionResult> CreateCityAsync([IdempotencyKey] Guid requestId, [FromBody] CreateCityDto dto, CancellationToken ct)
        {

            var response = await mediator.Send(new CreateCityCommand(requestId, dto), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{cityId:int}")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update City")]
        [EndpointSummary("Update an existing city")]
        [EndpointDescription("Updates city details and government assignment.")]
        public async Task<IActionResult> UpdateCityAsync([IdempotencyKey] Guid requestId, int cityId, [FromBody] UpdateCityDto dto, CancellationToken ct)
        {

            var response = await mediator.Send(new UpdateCityCommand(requestId, cityId, dto), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{cityId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete City")]
        [EndpointSummary("Delete a city")]
        [EndpointDescription("Deletes an existing city from the system.")]
        public async Task<IActionResult> DeleteCityAsync([IdempotencyKey] Guid requestId, int cityId, CancellationToken ct)
        {

            var response = await mediator.Send(new DeleteCityCommand(requestId, cityId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{cityId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin City")]
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
        [EndpointName("Get Admin Cities")]
        [EndpointSummary("Get cities")]
        [EndpointDescription("Returns paginated cities using the provided filter criteria.")]
        public async Task<IActionResult> CitiesAsync([FromQuery] CityFilterDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new FilterCitiesQuery(filter), ct);
            return response.ToActionResult();
        }
    }
}
