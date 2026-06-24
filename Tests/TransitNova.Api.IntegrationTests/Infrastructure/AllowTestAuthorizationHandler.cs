using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal sealed class AllowTestAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        foreach (var requirement in context.PendingRequirements.ToArray())
        {
            if (requirement is not DenyAnonymousAuthorizationRequirement)
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
