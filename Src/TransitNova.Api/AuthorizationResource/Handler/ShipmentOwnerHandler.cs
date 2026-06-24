using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
namespace TransitNova.Api.AuthorizationResource.Handler
{
    public class ShipmentOwnerHandler(IUserRulesRepository userRulesRepository) : AuthorizationHandler<ShipmentOwnerRequirement, Guid>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ShipmentOwnerRequirement requirement, Guid shipmentId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null)
                return;

            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return;

            //====== Authorization Check: Verify User Ownership of Shipment ======
            var isOwner = await userRulesRepository.OwnsShipmentAsync(currentUserId, shipmentId, CancellationToken.None);
            if (isOwner) context.Succeed(requirement);
           
        }

    }

}
