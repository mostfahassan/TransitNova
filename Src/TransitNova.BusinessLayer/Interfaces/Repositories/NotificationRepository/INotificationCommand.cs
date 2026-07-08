using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository
{
    public interface INotificationCommand
    {
        Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken);
        Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken);
    }
}