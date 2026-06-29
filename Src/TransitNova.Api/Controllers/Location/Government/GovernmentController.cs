using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Location.Governments.Queries;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Location.Government
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/governments")]
    [Tags("Governments")]
    public class GovernmentController(IMediator mediator) : ControllerBase
    {
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{governmentId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Government")]
        [EndpointSummary("Get government details")]
        [EndpointDescription("Returns government details by government identifier.")]
        public async Task<IActionResult> GovernmentAsync(int governmentId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetGovernmentByIdQuery(governmentId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Governments")]
        [EndpointSummary("Get all governments")]
        [EndpointDescription("Returns all governments in the system.")]
        public async Task<IActionResult> GovernmentsAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetGovernmentsQuery(), ct);
            return response.ToActionResult();
        }
    }
}
