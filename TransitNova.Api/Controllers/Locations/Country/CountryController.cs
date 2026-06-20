using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
namespace TransitNova.Api.Controllers.Locations.Country
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
        {
            var query = new GetCountriesQuery();
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result.Data);
        }
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{governmentId:int}/governments")]
        public async Task<IActionResult> GetCountryGovernments(int governmentId, CancellationToken cancellationToken)
        {
            var query = new GetCountryGovernmentsQuery(governmentId);
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result.Data);
        }
    }
}
