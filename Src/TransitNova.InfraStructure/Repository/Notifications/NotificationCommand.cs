using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.Notifications
{
    internal class NotificationCommand(AppDbContext context) : INotificationCommand
    {
        public async Task AddNotificationAsync(Domain.Entities.MainEntities.Notification notification, CancellationToken cancellationToken)
        {
            await context.Notifications.AddAsync(notification, cancellationToken);
        }

        public Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken) =>
            context.Notifications
                .Where(notification => notification.UserId == userId && !notification.IsRead)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(notification => notification.IsRead, true),
                    cancellationToken);
    }
}