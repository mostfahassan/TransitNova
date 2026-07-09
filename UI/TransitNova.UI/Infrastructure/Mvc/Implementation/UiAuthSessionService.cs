using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;
using TransitNovaUI.BusinessLayer.DTOs.RefreshToken;
using TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

namespace TransitNova.UI.Infrastructure.Mvc.Implementation;

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

    public async Task<string?> GetValidAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrWhiteSpace(accessToken))
            return await TryRefreshAsync(cancellationToken) ? GetAccessToken() : null;

        if (!IsExpiredOrNearExpiry(accessToken))
            return accessToken;

        return await TryRefreshAsync(cancellationToken) ? GetAccessToken() : null;
    }

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

    private static bool IsExpiredOrNearExpiry(string accessToken)
    {
        try
        {
            var tokenParts = accessToken.Split('.');
            if (tokenParts.Length < 2)
                return true;

            var payload = tokenParts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            var padding = 4 - (payload.Length % 4);
            if (padding is > 0 and < 4)
                payload = payload.PadRight(payload.Length + padding, '=');

            var payloadBytes = Convert.FromBase64String(payload);
            using var document = JsonDocument.Parse(payloadBytes);

            if (!document.RootElement.TryGetProperty("exp", out var expiresElement))
                return true;

            var expiresUnix = expiresElement.ValueKind == JsonValueKind.Number
                ? expiresElement.GetInt64()
                : long.Parse(expiresElement.GetString() ?? "0");

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresUnix);
            return expiresAt <= DateTimeOffset.UtcNow.AddMinutes(1);
        }
        catch
        {
            return true;
        }
    }

    private HttpContext HttpContext =>
        httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("HttpContext is not available.");

    private ISession Session => HttpContext.Session;
}