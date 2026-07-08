using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.Admin.Shipmments
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/v{version:apiVersion}/admin/shipments")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ShipmentsController(IMediator mediator) : ControllerBase
    {

        [Authorize(Policy = AdminPermissions.ViewShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Shipments For Admin")]
        [EndpointSummary("Get filtered shipments for the operation manager")]
        [EndpointDescription(
         "Returns the filtered shipments list available to the authenticated operation manager.")]
        public async Task<IActionResult> ShipmentsAsync([FromQuery] ShipmentFilterDto filter, CancellationToken ct)
        {
            var result = await mediator.Send(new FilterShipmentsQuery(filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{shipmentId:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Shipment For Admin")]
        [EndpointSummary("Get shipment details by id")]
        [EndpointDescription("Returns the full shipment details for the specified shipment identifier.")]

        public async Task<IActionResult> ShipmentAsync(Guid shipmentId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetShipmentByIdQuery(shipmentId), ct);
            return result.ToActionResult();
        }
    }
}
