using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Contracts.Keys;
using TransitNovaPayment.Busieness.Services.Payment.Command;
namespace TransitNovaPayment.API.Controllers.Payment
{
    [Route("api/payments")]
    [ApiController]
    [Tags("Payment")]
    public class PaymentController(IMediator mediator) : ControllerBase
    {
        [HttpPost("pay")]
        [EnableRateLimiting("CommandsLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Shipment Payment")]
        [EndpointSummary("Initiate Simulated Payment Process")]
        [EndpointDescription("Processes a simulated payment transaction for a specific shipment. \n\nSecurity Requirement: Requires a valid 'X-PaymentKey' in the header to authenticate the client request (simulating Public/Private key gateway validation)." +
            "Note: This endpoint simulates network latency and returns randomized Success/Failure outcomes for testing purposes.")]
        public async Task<IActionResult> Pay([FromHeader(Name = "X-PaymentKey")] string PublicKey , [FromBody]CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            if (!Symetric(PublicKey))
                return Unauthorized();

            var response = await mediator.Send(new CreatePaymentCommand(dto), cancellationToken);
            return Ok(response);
        }

        static bool Symetric(string publicKey)
           => PrivateKey.Key.Equals(publicKey);
    }
}
