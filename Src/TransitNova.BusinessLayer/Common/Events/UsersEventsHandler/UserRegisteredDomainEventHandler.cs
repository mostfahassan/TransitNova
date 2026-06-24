using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.UsersEvent;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.UsersEventsHandler
{
    public sealed class UserRegisteredDomainEventHandler(IAdminQueryRepository admins, INotificationCommand notificationRepo, IUnitOfWork unitOfWork) : INotificationHandler<UserRegisteredDomainEvent>
    {
        public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
        {
            var adminsIds = await admins.GetAdminIdsAsync(cancellationToken);
            var message = $"User registered successfully | Name: {notification.FullName} | " + $"Phone: {notification.PhoneNumber} | Role: {notification.Role}";
            foreach (var adminId in adminsIds)
            {
                var Adminsnotification = Notification.Create(adminId,"New Registered User", message);
                await notificationRepo.AddNotificationAsync(Adminsnotification, cancellationToken);

            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
