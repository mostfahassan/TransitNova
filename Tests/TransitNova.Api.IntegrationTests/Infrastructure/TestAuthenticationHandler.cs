using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
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
    internal static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(AuthenticationHeader))
            return Task.FromResult(AuthenticateResult.NoResult());

        var userName = Request.Headers.TryGetValue(UserHeader, out var value)
            ? value.ToString()
            : "integration-test-user";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, UserId.ToString()),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, "integration@transitnova.test"),
            new(ClaimTypes.Role, Role.Admin),
            new(ClaimTypes.Role, Role.User),
            new(ClaimTypes.Role, Role.Carrier),
            new(ClaimTypes.Role, Role.OperationManager)
        };

        claims.AddRange(UserPermissions.All.Select(permission => new Claim("Permission", permission)));
        claims.AddRange(CarrierPermissions.All.Select(permission => new Claim("Permission", permission)));
        claims.AddRange(OperationManagerPermissions.All.Select(permission => new Claim("Permission", permission)));
        claims.AddRange(AdminPermissions.All.Select(permission => new Claim("Permission", permission)));

        var identity = new ClaimsIdentity(claims, SchemeName, ClaimTypes.Name, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    public Task SignOutAsync(AuthenticationProperties? properties) => Task.CompletedTask;
}
