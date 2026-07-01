using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;

namespace TransitNova.Api.Controllers.Location.Country
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/countries")]
    [Tags("Countries")]
    public sealed class CountryController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Countries")]
        [EndpointSummary("Get all countries")]
        [EndpointDescription("Returns all countries in the system.")]
        public async Task<IActionResult> CountriesAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetCountriesQuery(), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{countryId:int}/governments")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Country Governments")]
        [EndpointSummary("Get governments for a country")]
        [EndpointDescription("Returns all governments that belong to the supplied country identifier.")]
        public async Task<IActionResult> CountryGovernmentsAsync(int countryId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetCountryGovernmentsQuery(countryId), ct);
            return response.ToActionResult();
        }
    }
}
