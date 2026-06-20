using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
namespace TransitNova.Api.Controllers.Locations.City
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController(IMediator mediator) : ControllerBase
    {

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{governmentId}/cities")]
        public async Task<IActionResult> GetCitiesByGovernment(int governmentId, CancellationToken cancellationToken)
        {
            var query = new GetCitiesByGovernmentQuery(governmentId);
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result.Data);
        }
    }
}
