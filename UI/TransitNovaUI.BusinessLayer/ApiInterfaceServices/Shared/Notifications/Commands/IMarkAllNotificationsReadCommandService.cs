namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Commands;

public interface IMarkAllNotificationsReadCommandService
{
    Task<ApiResponse> MarkAllAsReadAsync(string bearerToken, CancellationToken cancellationToken = default);
}