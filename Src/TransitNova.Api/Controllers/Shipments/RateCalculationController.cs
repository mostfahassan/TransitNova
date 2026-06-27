using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Commands;

namespace TransitNova.Api.Controllers.Shipments
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/shipments")]
    [Tags("Shipments")]
    public sealed class RateCalculationController(IMediator mediator) : ControllerBase
    {
        [HttpPost("rate-calculation")]
        [AllowAnonymous]
        [EnableRateLimiting("CommandsLimiter")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Calculate Shipment Rate")]
        [EndpointSummary("Calculate a shipment rate.")]
        [EndpointDescription("Calculates the estimated shipment rate from package dimensions, shipment type, and transportation mode. This endpoint is anonymous and does not create or mutate shipment data.")]
        public async Task<IActionResult> CalculateAsync([FromBody] RateCalculatorDto dto, CancellationToken ct)
        {
            var response = await mediator.Send(new RateCalculatorCommand(dto), ct);
            return response.ToActionResult();
        }
    }
}
