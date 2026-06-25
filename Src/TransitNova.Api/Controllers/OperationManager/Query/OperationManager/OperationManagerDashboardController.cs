using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.OperationManager.Query.OperationManager
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-manager/dashboard")]
    [Tags("Operation Manager Dashboard")]
    public sealed class OperationManagerDashboardController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = OperationManagerPermissions.ViewDashboard)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Dashboard")]
        [EndpointSummary("Get the operation manager dashboard")]
        [EndpointDescription("Returns the dashboard data for the authenticated operation manager, including the summary metrics required to monitor the system.")]
        public async Task<IActionResult> DashboardAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetOperationManagerDashboardQuery(), ct);
            return response.ToActionResult();
        }
    }
}