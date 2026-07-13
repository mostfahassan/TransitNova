using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
namespace TransitNova.Api.Controllers.OperationManager.Query.CarrierQueries
{
    [Authorize(Roles = Role.OperationManager)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-managers/carriers")]
    [Tags("Operation Manager Carriers")]
    public class OperationManagerCarrierController(IMediator mediator) : ControllerBase
    {
        
        [Authorize(Policy = OperationManagerPermissions.ViewCarriers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Carriers")]
        [EndpointSummary("Get carriers for operation management")]
        [EndpointDescription("Returns the list of carriers available to the authenticated operation manager using the provided filter criteria.")]
        public async Task<IActionResult> CarriersAsync([FromQuery] FilterCarrierDto filter, CancellationToken ct)
        {

            var result = await mediator.Send(new GetCarriersForOperationManagerQuery(filter), ct);
            return result.ToActionResult();
        }

       
        [Authorize(Policy = OperationManagerPermissions.ViewCarrierDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Operation Manager Carrier")]
        [EndpointSummary("Get specific carriers for operation management")]
        [EndpointDescription("Returns specific carriers available to the authenticated operation manager using the provided filter criteria.")]
        public async Task<IActionResult> CarrierAsync(Guid carrierId, CancellationToken ct)
        {

            var result = await mediator.Send(new GetCarrierProfileQuery(carrierId), ct);
            return result.ToActionResult();
        }


        [Authorize(Policy = OperationManagerPermissions.ViewAllShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipments For Operation Manager")]
        [EndpointSummary("Get authenticated carrier shipments for operation management")]
        [EndpointDescription("Returns the shipments assigned to the authenticated carrier using the provided filter criteria.")]
        public async Task<IActionResult> ShipmentsAsync(Guid carrierId, [FromQuery] CarrierShipmentFilterDto filter, CancellationToken ct)
        {

            var result = await mediator.Send(new GetCarrierShipmentsQuery(carrierId, filter), ct);
            return result.ToActionResult();
        }


        [Authorize(Policy = OperationManagerPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments/{shipmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipment ById For Operation Manager")]
        [EndpointSummary("Get details of a specific carrier shipment")]
        [EndpointDescription("Returns the detailed information for a shipment that belongs to the authenticated carrier.")]
        public async Task<IActionResult> ShipmentAsync(Guid shipmentId, Guid carrierId, CancellationToken ct)
        {

            var result = await mediator.Send(new GetCarrierShipmentDetailsQuery(carrierId, shipmentId), ct);
            return result.ToActionResult();
        }
    }
}
