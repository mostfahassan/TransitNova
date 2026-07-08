using TransitNova.BusinessLayer.DTOs.Notification;

namespace TransitNova.BusinessLayer.Interfaces.Services.Notification
{
    public interface INotificationBroadcaster
    {
        Task SendToUserAsync(
            Guid userId,
            NotificationDto notification,
            CancellationToken cancellationToken);
    }
}