using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.Payment.Command;
namespace TransitNova.Api.Controllers.Payment
{
    [Authorize]
    [Route("api/v{version:apiversion}/payments")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PaymentController(IMediator mediator) : ControllerBase
    {
       
        [HttpPost("process")]
        [EnableRateLimiting("CommandsLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [Consumes("application/json")]
        [EndpointName("ProcessPayment")]
        [EndpointSummary("Processes a payment transaction for a shipment.")]
        [EndpointDescription("Processes a simulated payment transaction for the specified shipment.The endpoint requires a valid 'X-Idempotency-Key' header to ensure idempotent payment execution and prevent duplicate transactions. " +
                "Repeated requests with the same idempotency key will return the previously stored response.")]
        public async Task<IActionResult> Pay([FromHeader(Name = "X-Idempotency-Key")] Guid idempotentKey, [FromBody] CreatePaymentDto createPaymentDto, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new CreatePaymentCommand(createPaymentDto, idempotentKey));
            return response.ToActionResult();
        }
    }
}
