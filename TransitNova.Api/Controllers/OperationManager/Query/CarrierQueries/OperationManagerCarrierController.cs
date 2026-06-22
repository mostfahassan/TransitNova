using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.Controllers.OperationManager.Query.CarrierQueries
{
    [Authorize(Roles = Role.OperationManagerOrAdmin)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/operation-managers/carriers")]
    [Tags("Operation Manager Carriers")]
    public class OperationManagerCarrierController(IMediator mediator,IAuthorizationService authorizationService) : ControllerBase
    {
        [Authorize(Policy = AdminPermissions.ViewCarriers)]
        [Authorize(Policy = OperationManagerPermissions.ViewCarriers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carriers")]
        [EndpointSummary("Get carriers for operation management")]
        [EndpointDescription("Returns the list of carriers available to the authenticated operation manager using the provided filter criteria.")]
        public async Task<IActionResult> Carriers([FromQuery] FilterCarrierDto filter, CancellationToken ct)
        {

            var result = await mediator.Send(new GetCarriersForOperationManagerCommand(filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewCarrierDetails)]
        [Authorize(Policy = OperationManagerPermissions.ViewCarrierDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier")]
        [EndpointSummary("Get specific carriers for operation management")]
        [EndpointDescription("Returns specific carriers available to the authenticated operation manager using the provided filter criteria.")]
        public async Task<IActionResult> Carrier(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierProfileQuery(carrierId), ct);
            return result.ToActionResult();
        }


        [Authorize(Policy = AdminPermissions.ViewShipments)]
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
        public async Task<IActionResult> Shipments(Guid carrierId, [FromQuery] CarrierShipmentFilterDto filter, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierShipmentsQuery(carrierId, filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewShipmentDetails)]
        [Authorize(Policy = OperationManagerPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments/{shipmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipment Details For Operation Manager")]
        [EndpointSummary("Get details of a specific carrier shipment")]
        [EndpointDescription("Returns the detailed information for a shipment that belongs to the authenticated carrier.")]
        public async Task<IActionResult> Shipment(Guid shipmentId, Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierShipmentDetailsQuery(carrierId, shipmentId), ct);
            return result.ToActionResult();
        }

        private async Task<bool> IsCarrierOwnerAsync(Guid carrierId)
        {
            var authorizationResult =
                await authorizationService.AuthorizeAsync(
                    User,
                    carrierId,
                    CarrierPermissions.IsCarrierOwner);

            return authorizationResult.Succeeded;
        }
    }
}
