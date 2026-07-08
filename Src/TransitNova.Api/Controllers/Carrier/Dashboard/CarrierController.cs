using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Carrier.Dashboard
{
    [Authorize(Roles = Role.Carrier)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/carriers")]
    [Tags("Carrier Dashboard")]
    public class CarrierController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = CarrierPermissions.CanViewDashboard)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Dashboard")]
        [EndpointSummary("Get the authenticated carrier dashboard")]
        [EndpointDescription("Returns the dashboard data for the authenticated carrier, including the information required to monitor the account and shipment activity.")]
        public async Task<IActionResult> DashboardAsync(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();
            var result = await mediator.Send(new GetCarrierDashboardQuery(carrierId), ct);
            return result.ToActionResult();
        }
        private async Task<bool> IsCarrierOwnerAsync(Guid carrierId)
        {
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, carrierId, CarrierPermissions.IsCarrierOwner);
            return authorizationResult.Succeeded;
        }
    }
}
