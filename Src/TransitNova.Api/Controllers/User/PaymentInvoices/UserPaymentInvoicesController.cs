using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.Features.UserOperations.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.User.PaymentInvoices
{
    [Authorize(Roles = Role.User)]
    [Route("api/v{version:apiVersion}/users/payment-invoices")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("User Payment Invoices")]
    public sealed class UserPaymentInvoicesController(IMediator mediator) : ControllerBase
    {
        [Authorize(Policy = UserPermissions.UserCanViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{paymentId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get User Payment Invoice")]
        [EndpointSummary("Retrieve a payment invoice for the authenticated user.")]
        [EndpointDescription("Returns a payment invoice only when it belongs to the authenticated user.")]
        public async Task<IActionResult> GetUserInvoiceAsync(Guid paymentId, CancellationToken ct)
        {
            var response = await mediator.Send(new GetUserPaymentInvoiceQuery(User.GetUserId(), paymentId), ct);
            return response.ToActionResult();
        }

        [Authorize(Policy = UserPermissions.UserCanViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get User Payment Invoices")]
        [EndpointSummary("Retrieve payment invoices for the authenticated user.")]
        [EndpointDescription("Returns all payment invoices that belong to the authenticated user.")]
        public async Task<IActionResult> GetUserInvoicesAsync(CancellationToken ct)
        {
            var response = await mediator.Send(new GetUserPaymentInvoicesQuery(User.GetUserId()), ct);
            return response.ToActionResult();
        }
    }
}
