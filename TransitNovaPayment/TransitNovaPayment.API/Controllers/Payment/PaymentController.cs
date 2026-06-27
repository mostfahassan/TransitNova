using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Services.Payment.Command;
using TransitNovaPayment.Busieness.Services.Payment.Query;
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
        public async Task<IActionResult> Pay([FromHeader(Name ="X-PaymentKey")] string PublicKey , [FromBody]CreatePaymentDto dto, CancellationToken cancellationToken)
        {
          
            var response = await mediator.Send(new CreatePaymentCommand(dto, PublicKey), cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("history")]
        [EnableRateLimiting("CommandsLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Payment History")]
        [EndpointSummary("Retrieves paginated payment history records.")]
        [EndpointDescription("Retrieves payment history records using the supplied filtering criteria.Supports filtering by payment ID, payment status, creator information, and date range. " +
            "Results are returned in a paginated format.")]
        public async Task<IActionResult> History([FromBody] FilterPaymentHistoryDto dto, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new FilterPaymentsQuery(dto), cancellationToken);
            return Ok(response);
        }
    }
}
