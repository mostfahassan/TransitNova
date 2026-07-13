using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Events.BundleEventsHandler
{
    public class BundleUpdatedDomainEventHandler(
        IBundleSubscriptionQueryRepository subscriptionRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<BundleUpdatedDomainEvent>
    {
        public async Task Handle(BundleUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var subscribedUsers = await subscriptionRepository.GetSubscribedUsersAsync(notification.Id, cancellationToken);

            foreach (var user in subscribedUsers)
            {
                var notificationCreated = Notification.Create(
                    user.Id,
                    "Bundle Updated",
                    $"Your subscribed bundle has been updated. The current price is {notification.BundlePrice:0.00}.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}