
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Api.Infrastructure.Idempotency;
namespace TransitNova.Api.Controllers.Carrier.Command
{
    [Authorize(Roles = Role.Carrier)]
    [Route("api/v{version:apiVersion}/carriers")]
    [ApiController]
    [ApiVersion("1.0")]
    [Tags("Carrier Commands")]
    public class CarrierController(IMediator mediator ,IAuthorizationService authorizationService) : ControllerBase
    {
        [Authorize(Policy = CarrierPermissions.UpdateProfile)]
        [Authorize(Policy = CarrierPermissions.HasCompletedProfile)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("profile")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Update Carrier Profile")]
        [EndpointSummary("Update Authenticated Carrier Profile")]
        [EndpointDescription("Updates the primary profile details for the authenticated carrier. Requires a valid X-Idempotency-Key header and resource ownership validation to ensure safe and idempotent updates.")]
        public async Task<IActionResult> UpdateProfileAsync([IdempotencyKey] Guid requestId,  [FromBody] UpdateCarrierDto dto, CancellationToken ct)
        {

            if (!await IsCarrierOwnerAsync(dto.Id))
                return Forbid();

            var userId = User.GetUserId();
            var response = await mediator.Send(new UpdateCarrierProfileCommand(requestId, userId, dto), ct);
            return response.ToActionResult();
        }


        [Authorize(Policy = CarrierPermissions.CanAddAdditionalInfo)]
        [EnableRateLimiting("CommandsLimiter")]
        [HttpPut("additional-info")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [EndpointName("Add Carrier Additional Info")]
        [EndpointSummary("Add Carrier Additional Info For Authenticated Carrier")]
        [EndpointDescription("Appends or updates supplementary information for the authenticated carrier. Requires a valid X-Idempotency-Key header and resource ownership validation to ensure safe execution")]
        public async Task<IActionResult> AddCarrierInfoAsync([IdempotencyKey] Guid requestId, [FromBody] AdditionalInfoDto dto, CancellationToken ct)
        {

            if (!await IsCarrierOwnerAsync(dto.Id))
                return Forbid();
            var userId = User.GetUserId();
            var response = await mediator.Send(new AddingCarrierAdditionalInfoCommand(requestId, dto , userId), ct);
            return response.ToActionResult();
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
