using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Events.BundleEventsHandler
{
    public class UserUnsubscribedFromBundleDomainEventHandler(
        IUserQueryRepository userRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<UserUnsubscribedFromBundleDomainEvent>
    {
        public async Task Handle(UserUnsubscribedFromBundleDomainEvent notification, CancellationToken cancellationToken)
        {
            var appUserId = await userRepository.GetAppUserIdByProfileIdAsync(notification.UserProfileId, cancellationToken);
            if (!appUserId.HasValue)
                return;

            var notificationCreated = Notification.Create(appUserId.Value, "Bundle Subscription Ended", "Your bundle subscription has been cancelled.");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}