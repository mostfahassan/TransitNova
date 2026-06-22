using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.Api.AuthorizationResource.Handler
{
    public class CarrierOwnerRequirement : IAuthorizationRequirement
    {
    }
    public class CarrierOwnerHandler(ICarrierRulesRepository carrierRepository) : AuthorizationHandler<CarrierOwnerRequirement, Guid>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CarrierOwnerRequirement requirement, Guid carrierId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null)
                return;

            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return;

            var isCarrierOwner = await carrierRepository.IsCarrierOwnerAsync(carrierId, currentUserId);

            if (isCarrierOwner) context.Succeed(requirement);

          
        }
    }

}
