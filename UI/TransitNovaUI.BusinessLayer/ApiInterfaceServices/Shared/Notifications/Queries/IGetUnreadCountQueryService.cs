using TransitNovaUI.BusinessLayer.DTOs.Notification;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Queries;

public interface IGetUnreadCountQueryService
{
    Task<ApiResponse<UiUnreadCountDto>> GetUnreadCountAsync(string bearerToken, CancellationToken cancellationToken = default);
}