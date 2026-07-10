using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.Api.Controllers;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Features.Roles.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.Admin.RolesManagement
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/roles")]
    [Tags("Admin Roles Management")]
    public sealed class RolesController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewRoles)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Roles")]
        [EndpointSummary("Get all roles")]
        [EndpointDescription("Returns all roles available in the system.")]
        public async Task<IActionResult> RolesAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetRolesQuery(), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewRoleDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{roleId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Role")]
        [EndpointSummary("Get role details")]
        [EndpointDescription("Returns paginated member details for a specific role.")]
        public async Task<IActionResult> RoleDetailsAsync(Guid roleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var response = await mediator.Send(new GetRoleByIdQuery(roleId, pageNumber, pageSize), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewRoleDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{roleId:guid}/members")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Role Members")]
        [EndpointSummary("Get role members")]
        [EndpointDescription("Returns all users with their membership state for a specific role.")]
        public async Task<IActionResult> RoleMembersAsync(Guid roleId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetRoleMembersQuery(roleId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.CreateRole)]
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
        [EndpointName("Create Role")]
        [EndpointSummary("Create a new role")]
        [EndpointDescription("Creates a new role in the system.")]
        public async Task<IActionResult> CreateRoleAsync([IdempotencyKey] Guid requestId, [FromBody] RoleNameDto dto, CancellationToken ct)
        {
            var response = await mediator.Send(new CreateRoleCommand(requestId, dto.RoleName), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.UpdateRole)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{roleId:guid}")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Role")]
        [EndpointSummary("Update an existing role")]
        [EndpointDescription("Updates the name of an existing role.")]
        public async Task<IActionResult> UpdateRoleAsync([IdempotencyKey] Guid requestId, Guid roleId, [FromBody] RoleNameDto dto, CancellationToken ct)
        {
            var response = await mediator.Send(new UpdateRoleCommand(requestId, roleId, dto.RoleName), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.DeleteRole)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{roleId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Role")]
        [EndpointSummary("Delete a role")]
        [EndpointDescription("Deletes an existing role from the system.")]
        public async Task<IActionResult> DeleteRoleAsync([IdempotencyKey] Guid requestId, Guid roleId, CancellationToken ct)
        {

            var response = await mediator.Send(new DeleteRoleCommand(requestId, roleId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ManageRoleMembers)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("{roleId:guid}/members")]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Update Role Members")]
        [EndpointSummary("Update role members")]
        [EndpointDescription("Synchronizes the users assigned to a role using the submitted membership state.")]
        public async Task<IActionResult> UpdateRoleMembersAsync([IdempotencyKey] Guid requestId, Guid roleId, [FromBody] UpdateRoleMembersDto dto, CancellationToken ct)
        {

            var response = await mediator.Send(new UpdateRoleMembersCommand(requestId, roleId, dto.Users), ct);
            return response.ToActionResult();
        }
    }
}
