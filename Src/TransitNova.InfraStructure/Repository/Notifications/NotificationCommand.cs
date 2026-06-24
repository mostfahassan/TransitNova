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
    }
}
