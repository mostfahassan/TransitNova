using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.CarrierCompany;
using TransitNova.BusinessLayer.Features.CarrierCompanies.Commands;
using TransitNova.BusinessLayer.Features.CarrierCompanies.Queries;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Admin.CarrierOperation
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/v{version:apiVersion}/admins/carriers")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("Admin Carriers")]
    public class AdminsCarrierOperationController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = Role.Admin)]
        [Authorize(Policy = AdminPermissions.DeleteCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{id:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Carrier")]
        [EndpointSummary("Delete a carrier")]
        [EndpointDescription("Deletes an existing carrier from the system.")]
        public async Task<IActionResult> DeleteCarrier([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid id, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new DeleteCarrierCommand(parsedRequestId, id, adminId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpPost("companies")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Create Carrier Company")]
        [EndpointSummary("Create a new carrier company")]
        [EndpointDescription("Creates a new carrier company in the system.")]
        public async Task<IActionResult> CreateCarrierCompany([FromHeader(Name = "X-Idempotency-Key")] string requestId, [FromBody] AddCarrierCompany dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            var response = await mediator.Send(new CreateCarrierCompanyCommand(parsedRequestId, dto, adminId), ct);
            return response.ToActionResult();
        }

        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("companies/{companyId:guid}")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Carrier Company")]
        [EndpointSummary("Update an existing carrier company")]
        [EndpointDescription("Updates carrier company details.")]
        public async Task<IActionResult> UpdateCarrierCompany([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid companyId, [FromBody] UpdateCarrierCompany dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var adminId = User.GetUserId();
            dto.Id = companyId;

            var response = await mediator.Send(new UpdateCarrierCompanyCommand(parsedRequestId, companyId, dto, adminId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.DeleteCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("companies/{companyId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Carrier Company")]
        [EndpointSummary("Delete a carrier company")]
        [EndpointDescription("Deletes an existing carrier company from the system.")]
        public async Task<IActionResult> DeleteCarrierCompany([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid companyId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new DeleteCarrierCompanyCommand(parsedRequestId, companyId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewCarriers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("companies")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Companies")]
        [EndpointSummary("Get all carrier companies")]
        [EndpointDescription("Returns all carrier companies in the system.")]
        public async Task<IActionResult> CarrierCompanies(CancellationToken ct)
        {
            var response = await mediator.Send(new GetCarrierCompanyListQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewCarrierDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("companies/{companyId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Company")]
        [EndpointSummary("Get carrier company details")]
        [EndpointDescription("Returns carrier company details by carrier company identifier.")]
        public async Task<IActionResult> CarrierCompany(Guid companyId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetCarrierCompanyByIdQuery(companyId), ct);
            return response.ToActionResult();
        }
    }
}
