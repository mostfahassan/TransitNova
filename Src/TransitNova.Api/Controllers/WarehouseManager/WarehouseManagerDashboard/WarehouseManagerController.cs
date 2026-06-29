using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Commands;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.WarehouseManager.WarehouseManagerDashboard
{
    [Authorize(Roles = Role.WarehouseManager)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/warehouse-managers")]
    [Tags("Warehouse Manager Dashboard")]
    public sealed class WarehouseManagerController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = WarehouseManagerPermissions.ViewDashboard)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("dashboard")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Warehouse Manager Dashboard")]
        [EndpointSummary("Get warehouse manager dashboard")]
        [EndpointDescription("Returns the dashboard data for the authenticated warehouse manager.")]
        public async Task<IActionResult> DashboardAsync(CancellationToken ct)
        {
            var managerId = User.GetUserId();
            var response = await mediator.Send(new GetWarehouseManagerDashboardQuery(managerId), ct);
            return response.ToActionResult();
        }
        [Authorize(Policy = WarehouseManagerPermissions.Update)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("update")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Warehouse Manager")]
        [EndpointSummary("Update warehouse manager")]
        [EndpointDescription("Updates warehouse manager profile fields and warehouse assignment.")]
        public async Task<IActionResult> UpdateWarehouseManagerAsync([FromBody] UpdateWarehouseManagerProfile dto, CancellationToken ct)
        {
            var response = await mediator.Send(new UpdateWarehouseManagerCommand(dto), ct);
            return response.ToActionResult();
        }
    }
}
