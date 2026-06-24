using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.BundleEventsHandler
{
    public class UserSubscribedToBundleDomainEventHandler(INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<UserSubscribedToBundleDomainEvent>
    {
        public async Task Handle(UserSubscribedToBundleDomainEvent notification, CancellationToken cancellationToken)
        {
            var notificationCreated = Notification.Create(notification.Id, "Bundle Subscription Active", "Your bundle subscription is now active.");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
