using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Admin.User
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/users")]
    [Tags("Admin Users")]
    public sealed class AdminUsersController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewUsers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Users")]
        [EndpointSummary("Get users")]
        [EndpointDescription("Returns paginated users using the provided filter criteria.")]
        public async Task<IActionResult> Users([FromQuery] UserFiltrationDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new FilterUsersCommand(filter), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewUserDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{userId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get User Details")]
        [EndpointSummary("Get user details")]
        [EndpointDescription("Returns full details for a specific user.")]
        public async Task<IActionResult> UserDetails(Guid userId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetAdminUserDetailsQuery(userId), ct);
            return response.ToActionResult();
        }
    }
}
