using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder),
      IAuthenticationSignOutHandler
{
    internal const string SchemeName = "IntegrationTest";
    internal const string AuthenticationHeader = "X-Test-Authentication";
    internal const string UserHeader = "X-Test-User";
    internal const string RolesHeader = "X-Test-Roles";
    internal const string PermissionsHeader = "X-Test-Permissions";
    internal const string BypassAuthorizationHeader = "X-Test-Bypass-Authorization";
    internal static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(AuthenticationHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        var userName = Request.Headers.TryGetValue(UserHeader, out var value)
            ? value.ToString()
            : "integration-test-user";

        var roles = Request.Headers.TryGetValue(RolesHeader, out var rolesHeader)
            ? ParseValues(rolesHeader.ToString())
            : new[] { Role.Admin, Role.User, Role.Carrier, Role.OperationManager };

        var permissions = Request.Headers.TryGetValue(PermissionsHeader, out var permissionsHeader)
            ? ParseValues(permissionsHeader.ToString())
            : UserPermissions.All
                .Concat(CarrierPermissions.All)
                .Concat(OperationManagerPermissions.All)
                .Concat(AdminPermissions.All)
                .Distinct(StringComparer.Ordinal);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, UserId.ToString()),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, "integration@transitnova.test")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

        var identity = new ClaimsIdentity(claims, SchemeName, ClaimTypes.Name, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    public Task SignOutAsync(AuthenticationProperties? properties) => Task.CompletedTask;

    private static IEnumerable<string> ParseValues(string value)
    {
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}

internal sealed class AllowTestAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        if (!ShouldBypassAuthorization(context))
            return Task.CompletedTask;

        foreach (var requirement in context.PendingRequirements.ToArray())
        {
            if (requirement is not DenyAnonymousAuthorizationRequirement)
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool ShouldBypassAuthorization(AuthorizationHandlerContext context)
    {
        if (context.PendingRequirements.OfType<RefreshTokenOwnershipRequirement>().Any())
            return false;

        HttpContext? httpContext = context.Resource switch
        {
            HttpContext currentContext => currentContext,
            AuthorizationFilterContext filterContext => filterContext.HttpContext,
            _ => null
        };

        if (httpContext is null)
            return true;

        if (!httpContext.Request.Headers.TryGetValue(TestAuthenticationHandler.BypassAuthorizationHeader, out var value))
            return true;

        return !string.Equals(value.ToString(), "false", StringComparison.OrdinalIgnoreCase);
    }
}
