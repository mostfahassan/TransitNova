using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
namespace TransitNova.Api.AuthorizationResource.Handler
{

    public class RefreshTokenAuthenticationHandler(IRefreshTokenRepository refreshToken)
        : AuthorizationHandler<RefreshTokenAuthenticationRequirement, string>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RefreshTokenAuthenticationRequirement requirement, string resource)

        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out Guid userId)) return;

            var owner = await refreshToken.UserOwnsRefreshTokenAsync(userId, resource, CancellationToken.None);

            if (owner)
            {
                context.Succeed(requirement);
            }
        }
    }

}
