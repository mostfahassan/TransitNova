using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
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
    public sealed class CarrierQueriesController(IMediator mediator, IAuthorizationService authorizationService) : ControllerBase
    {
        [Authorize(Policy = CarrierPermissions.CanViewDashboard)]
        [EnableRateLimiting("DefaultRateLimiter")]
        [HttpGet("{carrierId:guid}/dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("Get Carrier Dashboard")]
        [EndpointSummary("Get the authenticated carrier dashboard")]
        [EndpointDescription("Returns the dashboard data for the authenticated carrier, including the information required to monitor the account and shipment activity.")]
        public async Task<IActionResult> Dashboard(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();
            var result = await mediator.Send(new GetCarrierDashboardQuery(carrierId), ct);
            return result.ToActionResult();
        }

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
        public async Task<IActionResult> Shipments(Guid carrierId, [FromQuery] CarrierShipmentFilterDto filter, CancellationToken ct)
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
        [EndpointName("Get Carrier Shipment Details")]
        [EndpointSummary("Get details of a specific carrier shipment")]
        [EndpointDescription("Returns the detailed information for a shipment that belongs to the authenticated carrier.")]
        public async Task<IActionResult> Shipment(Guid shipmentId,Guid carrierId, CancellationToken ct)
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
        [EndpointName("Get Carrier Rating")]
        [EndpointSummary("Get the authenticated carrier rating")]
        [EndpointDescription("Returns the rating information for the authenticated carrier.")]
        public async Task<IActionResult> Profile(Guid carrierId,CancellationToken ct)
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
        [EndpointName("Get Carrier Revenue")]
        [EndpointSummary("Get the authenticated carrier revenue")]
        [EndpointDescription("Returns the revenue information for the authenticated carrier.")]
        public async Task<IActionResult> Rating(Guid carrierId, CancellationToken ct)
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
        [EndpointName("Get Carrier Profile")]
        [EndpointSummary("Get the authenticated carrier profile")]
        [EndpointDescription("Returns the profile information for the authenticated carrier.")]
        public async Task<IActionResult> Revenue(Guid carrierId, CancellationToken ct)
        {
            if (!await IsCarrierOwnerAsync(carrierId))
                return Forbid();

            var result = await mediator.Send(new GetCarrierRevenueQuery(carrierId), ct);
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
