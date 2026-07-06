using TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

namespace TransitNova.UI.Infrastructure.Mvc.Interface;

public interface IUiAuthSessionService
{
    string? GetAccessToken();
    string? GetRefreshToken();
    Guid? GetUserId();
    string? GetCurrentRole();
    string? GetWarehouseId();
    void SetWarehouseId(Guid warehouseId);
    Task SignInAsync(UiAuthResponseDto authResponse, CancellationToken cancellationToken = default);
    Task<bool> TryRefreshAsync(CancellationToken cancellationToken = default);
    Task SignOutAsync();
}
