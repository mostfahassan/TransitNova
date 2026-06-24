using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.Api.AuthorizationResource.Requirement;
namespace TransitNova.Api.AuthorizationResource.Handler
{
    public class TokenOwnerHandler() : AuthorizationHandler<RefreshTokenOwnershipRequirement, Guid>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, RefreshTokenOwnershipRequirement requirement, Guid routeId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var currentUserId))
                return;

            if (currentUserId == routeId)  
              context.Succeed(requirement);
        }
    }
}
