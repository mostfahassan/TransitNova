using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.Api.AuthorizationResource.Handler
{
    public class CompletedProfileHandler(ICarrierRulesRepository carrier) : AuthorizationHandler<CompletedProfileRequirement>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompletedProfileRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null)
                return;

            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return;

            //===== Check If Carrier Completed His Profile Info To Get Ready
            var hasCompletedProfile = await carrier.IsCarrierHasCompletedProfileAsync(currentUserId);
            if (hasCompletedProfile) context.Succeed(requirement);

        }
    }

}
