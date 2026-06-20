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
        public async Task<IActionResult> Roles(CancellationToken ct)
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
        [EndpointDescription("Returns details for a specific role.")]
        public async Task<IActionResult> RoleDetails(Guid roleId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetRoleByIdQuery(roleId), ct);
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
        public async Task<IActionResult> RoleMembers(Guid roleId, CancellationToken ct)
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
        public async Task<IActionResult> CreateRole([FromHeader(Name = "X-Idempotency-Key")] string requestId, [FromBody] RoleNameDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new CreateRoleCommand(parsedRequestId, dto.RoleName), ct);
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
        public async Task<IActionResult> UpdateRole([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid roleId, [FromBody] RoleNameDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new UpdateRoleCommand(parsedRequestId, roleId, dto.RoleName), ct);
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
        public async Task<IActionResult> DeleteRole([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid roleId, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(new DeleteRoleCommand(parsedRequestId, roleId), ct);
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
        public async Task<IActionResult> UpdateRoleMembers([FromHeader(Name = "X-Idempotency-Key")] string requestId, Guid roleId, [FromBody] UpdateRoleMembersDto dto, CancellationToken ct)
        {
            if (!Guid.TryParse(requestId, out Guid parsedRequestId))
                return BadRequest();

            var response = await mediator.Send(
                new UpdateRoleMembersCommand(parsedRequestId, roleId, dto.Users), ct);
            return response.ToActionResult();
        }
    }
}
