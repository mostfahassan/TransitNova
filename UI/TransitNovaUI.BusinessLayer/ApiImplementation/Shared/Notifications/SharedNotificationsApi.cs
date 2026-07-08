using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Notification;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Notifications;

public sealed class SharedNotificationsApi(IHttpHandler httpHandler, HttpClient httpClient)
    : ApiServiceBase(httpHandler, httpClient), ISharedNotificationsQuery, ISharedNotificationsCommand
{
    public Task<ApiResponse<UiPagedResult<UiNotificationDto>>> GetNotificationsAsync(int pageNumber, int pageSize, string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(
            ApiRoutes.Notifications.GetNotificationsUrl,
            ("pageNumber", pageNumber),
            ("pageSize", pageSize)));

        return SendQueryRequestAsync<UiPagedResult<UiNotificationDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }

    public Task<ApiResponse<UiUnreadCountDto>> GetUnreadCountAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Notifications.GetUnreadCountUrl));
        return SendQueryRequestAsync<UiUnreadCountDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }

    public Task<ApiResponse> MarkAllAsReadAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Notifications.MarkAllAsReadUrl));
        return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, null, null);
    }
}