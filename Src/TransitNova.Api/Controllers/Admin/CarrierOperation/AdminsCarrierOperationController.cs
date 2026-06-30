using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.Admin.CarrierOperation
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/v{version:apiVersion}/admin/carriers")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("Admin Carriers")]
    public class AdminsCarrierOperationController(IMediator mediator) : ControllerBase
    {

        [Authorize(Policy = AdminPermissions.DeleteCarrier)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpDelete("{id:guid}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Delete Carrier")]
        [EndpointSummary("Delete a carrier")]
        [EndpointDescription("Deletes an existing carrier from the system.")]
        public async Task<IActionResult> DeleteCarrierAsync([IdempotencyKey] Guid requestId, Guid id, CancellationToken ct)
        {
            var adminId = User.GetUserId();
            var response = await mediator.Send(new DeleteCarrierCommand(requestId, id, adminId), ct);
            return response.ToActionResult();
        }


        [Authorize(Policy = AdminPermissions.ViewCarriers)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Carriers")]
        [EndpointSummary("Get carriers for Admin")]
        [EndpointDescription("Returns the list of carriers available to the authenticated Admin using the provided filter criteria.")]
        public async Task<IActionResult> CarriersAsync([FromQuery] FilterCarrierDto filter, CancellationToken ct)
        {
            var result = await mediator.Send(new GetCarriersForOperationManagerQuery(filter), ct);
            return result.ToActionResult();
        }


        [Authorize(Policy = AdminPermissions.ViewCarrierDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Admin Carrier")]
        [EndpointSummary("Get specific carriers for Admin")]
        [EndpointDescription("Returns specific carriers available to the authenticated Admin using the provided filter criteria.")]
        public async Task<IActionResult> CarrierAsync(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierProfileQuery(carrierId), ct);
            return result.ToActionResult();
        }



        [Authorize(Policy = AdminPermissions.ViewShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipments For Admin")]
        [EndpointSummary("Get authenticated carrier shipments for Admin")]
        [EndpointDescription("Returns the shipments assigned to the authenticated carrier using the provided filter criteria.")]
        public async Task<IActionResult> ShipmentsAsync(Guid carrierId, [FromQuery] CarrierShipmentFilterDto filter, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierShipmentsQuery(carrierId, filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = AdminPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments/{shipmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipment Details For Admin")]
        [EndpointSummary("Get details of a specific carrier shipment")]
        [EndpointDescription("Returns the detailed information for a shipment that belongs to the authenticated carrier.")]
        public async Task<IActionResult> ShipmentAsync(Guid shipmentId, Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierShipmentDetailsQuery(carrierId, shipmentId), ct);
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
