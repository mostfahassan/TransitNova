using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;
using TransitNovaUI.BusinessLayer.DTOs.RefreshToken;
using TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

namespace TransitNova.UI.Infrastructure.Mvc;

public sealed class UiAuthSessionService(
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenCommandService refreshTokenCommand)
    : IUiAuthSessionService
{
    public string? GetAccessToken() => Session.GetString(SessionKeys.AccessToken);

    public string? GetRefreshToken() => Session.GetString(SessionKeys.RefreshToken);

    public Guid? GetUserId()
    {
        var value = Session.GetString(SessionKeys.UserId);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    public string? GetCurrentRole() =>
        Session.GetString(SessionKeys.Roles)?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();

    public string? GetWarehouseId() => Session.GetString(SessionKeys.WarehouseId);

    public void SetWarehouseId(Guid warehouseId) =>
        Session.SetString(SessionKeys.WarehouseId, warehouseId.ToString());

    public async Task SignInAsync(UiAuthResponseDto authResponse, CancellationToken cancellationToken = default)
    {
        StoreSession(authResponse);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, authResponse.Id.ToString()),
            new(ClaimTypes.Name, authResponse.Username),
            new(ClaimTypes.Email, authResponse.Email),
            new("UserType", authResponse.UserType)
        };

        claims.AddRange(authResponse.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }

    public async Task<bool> TryRefreshAsync(CancellationToken cancellationToken = default)
    {
        var refreshToken = GetRefreshToken();
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var response = await refreshTokenCommand.RefreshTokenAsync(
            new UiRefreshTokenDto(refreshToken),
            cancellationToken);

        if (response.IsFailure || response.Data is null || !response.Data.IsAuthenticated)
        {
            await SignOutAsync();
            return false;
        }

        await SignInAsync(response.Data, cancellationToken);
        return true;
    }

    public async Task SignOutAsync()
    {
        Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private void StoreSession(UiAuthResponseDto authResponse)
    {
        Session.SetString(SessionKeys.AccessToken, authResponse.Token);
        Session.SetString(SessionKeys.RefreshToken, authResponse.RefreshToken);
        Session.SetString(SessionKeys.UserId, authResponse.Id.ToString());
        Session.SetString(SessionKeys.Username, authResponse.Username);
        Session.SetString(SessionKeys.Email, authResponse.Email);
        Session.SetString(SessionKeys.UserType, authResponse.UserType);
        Session.SetString(SessionKeys.Roles, string.Join(',', authResponse.Roles));
    }

    private HttpContext HttpContext =>
        httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("HttpContext is not available.");

    private ISession Session => HttpContext.Session;
}
