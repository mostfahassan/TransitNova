using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Notification;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Queries;

public interface IGetNotificationsQueryService
{
    Task<ApiResponse<UiPagedResult<UiNotificationDto>>> GetNotificationsAsync(int pageNumber, int pageSize, string bearerToken, CancellationToken cancellationToken = default);
}