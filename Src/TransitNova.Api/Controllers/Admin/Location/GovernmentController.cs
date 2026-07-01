using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Governments.Commands;
using TransitNova.BusinessLayer.Features.Location.Governments.Queries;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.Admin.Location
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/governments")]
    [Tags("Admin Governments")]
    public sealed class GovernmentController(IMediator mediator) : ControllerBase
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
        [EndpointName("Create Government")]
        [EndpointSummary("Create a new government")]
        [EndpointDescription("Creates a new government under a country.")]
        public async Task<IActionResult> CreateGovernmentAsync([IdempotencyKey] Guid requestId, [FromBody] CreateGovernmentDto dto, CancellationToken ct)
        {
            var response = await mediator.Send(new CreateGovernmentCommand(requestId, dto.Name, dto.CountryId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{governmentId:int}")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Government")]
        [EndpointSummary("Update an existing government")]
        [EndpointDescription("Updates government details and country assignment.")]
        public async Task<IActionResult> UpdateGovernmentAsync([IdempotencyKey] Guid requestId, int governmentId, [FromBody] UpdateGovernmentDto dto, CancellationToken ct)
        {

            var response = await mediator.Send(new UpdateGovernmentCommand(requestId, governmentId, dto.Name, dto.CountryId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{governmentId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Government")]
        [EndpointSummary("Delete a government")]
        [EndpointDescription("Deletes an existing government from the system.")]
        public async Task<IActionResult> DeleteGovernmentAsync([IdempotencyKey] Guid requestId, int governmentId, CancellationToken ct)
        {

            var response = await mediator.Send(new DeleteGovernmentCommand(requestId, governmentId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{governmentId:int}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Government")]
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
        [EndpointName("Get Admin Governments")]
        [EndpointSummary("Get all governments")]
        [EndpointDescription("Returns all governments in the system.")]
        public async Task<IActionResult> GovernmentsAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetGovernmentsQuery(), ct);
            return response.ToActionResult();
        }
    }
}
