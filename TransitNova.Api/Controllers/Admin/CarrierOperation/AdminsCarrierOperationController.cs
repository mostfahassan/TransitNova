using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Admin.CarrierOperation
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/v{version:apiVersion}/admin/carriers")]
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
    }
}
