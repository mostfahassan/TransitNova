
namespace TransitNova.BusinessLayer.Interfaces.Services.Notification
{
    public interface INotificationBroadcaster
    {
        Task SendToUserAsync(
        Guid userId,
        string title,
        string message,
        CancellationToken cancellationToken);
    }
}
