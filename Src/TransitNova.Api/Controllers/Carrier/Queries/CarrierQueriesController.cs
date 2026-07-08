using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.Api.Controllers.Carrier.Queries
{
    [Authorize(Roles = Role.Carrier)]
    [Authorize(Policy = CarrierPermissions.HasCompletedProfile)]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/carriers")]
    [Tags("Carrier Queries")]
    public sealed class CarrierQueriesController(IMediator mediator) : ControllerBase
    {

        [Authorize(Policy = CarrierPermissions.ViewAssignedShipments)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipments")]
        [EndpointSummary("Get shipments for the authenticated carrier")]
        [EndpointDescription("Returns the shipments assigned to the authenticated carrier using the provided filter criteria.")]
        public async Task<IActionResult> ShipmentsAsync(Guid carrierId, [FromQuery] CarrierShipmentFilterDto filter, CancellationToken ct)
        {

            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierShipmentsQuery(carrierId, filter), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = CarrierPermissions.ViewShipmentDetails)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/shipments/{shipmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Shipment ById")]
        [EndpointSummary("Get details of a specific carrier shipment")]
        [EndpointDescription("Returns the detailed information for a shipment that belongs to the authenticated carrier.")]
        public async Task<IActionResult> ShipmentAsync(Guid shipmentId,Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierShipmentDetailsQuery(carrierId, shipmentId), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = CarrierPermissions.ViewProfile)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Profile")]
        [EndpointSummary("Get the authenticated carrier profile")]
        [EndpointDescription("Returns the profile information for the authenticated carrier.")]
        public async Task<IActionResult> ProfileAsync(Guid carrierId,CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierProfileQuery(carrierId), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = CarrierPermissions.ViewRating)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Rating")]
        [EndpointSummary("Get the authenticated carrier rating")]
        [EndpointDescription("Returns the rating information for the authenticated carrier.")]
        public async Task<IActionResult> RatingAsync(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierRatingQuery(carrierId), ct);
            return result.ToActionResult();
        }


        [Authorize(Policy = CarrierPermissions.CanViewRevenue)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/revenue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Revenue")]
        [EndpointSummary("Get the authenticated carrier revenue")]
        [EndpointDescription("Returns the revenue information for the authenticated carrier.")]
        public async Task<IActionResult> RevenueAsync(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierRevenueQuery(carrierId), ct);
            return result.ToActionResult();
        }

        [Authorize(Policy = CarrierPermissions.CanViewRevenue)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/vehicles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Vehicle")]
        [EndpointSummary("Get the authenticated carrier vehicle")]
        [EndpointDescription("Returns the vehicle information for the authenticated carrier.")]
        public async Task<IActionResult> CarrierVehicle(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierVehicleQuery(carrierId), ct);
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
