
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Admin.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Admin.AdminDashboard
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/v{version:apiVersion}/admin/dashboard")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AdminsController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewDashboard)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Dashboard")]
        [EndpointSummary("Get the Admin dashboard")]
        [EndpointDescription("Returns the dashboard data for the authenticated Admin, including the summary metrics required to monitor the system.")]
        public async Task<IActionResult> DashboardAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetAdminDashboardQuery(), ct);
            return response.ToActionResult();
        }
    }
}
