using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.Admin.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.Admin.PaymentHistory
{
    [Authorize(Roles = Role.Admin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/payment-histories")]
    [Tags("Admin Payment Histories")]
    public sealed class PaymentHistoriesController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewPaymentHistories)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Payment Histories")]
        [EndpointSummary("Get paginated payment histories for admin users.")]
        [EndpointDescription("Returns filtered payment history records for admin users through the payment service integration.")]
        public async Task<IActionResult> GetAsync([FromQuery] PaymentHistoryFilterDto filter, CancellationToken ct)
        {
            var response = await mediator.Send(new GetAdminPaymentHistoriesQuery(filter), ct);
            return response.ToActionResult();
        }
    }
}
