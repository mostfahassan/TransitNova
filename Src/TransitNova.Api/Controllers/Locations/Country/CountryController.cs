using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
namespace TransitNova.Api.Controllers.Locations.Country
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    public class CountryController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCountriesAsync(CancellationToken cancellationToken)
        {
            var query = new GetCountriesQuery();
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result.Data);
        }
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{countryId:int}/governments")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCountryGovernmentsAsync(int countryId, CancellationToken cancellationToken)
        {
            var query = new GetCountryGovernmentsQuery(countryId);
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result.Data);
        }
    }
}
