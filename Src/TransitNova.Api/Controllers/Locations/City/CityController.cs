using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
namespace TransitNova.Api.Controllers.Locations.City
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/governments")]
    public class CityController(IMediator mediator) : ControllerBase
    {

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{governmentId:int}/cities")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCitiesByGovernmentAsync(int governmentId, CancellationToken cancellationToken)
        {
            var query = new GetCitiesByGovernmentQuery(governmentId);
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result.Data);
        }
    }
}
