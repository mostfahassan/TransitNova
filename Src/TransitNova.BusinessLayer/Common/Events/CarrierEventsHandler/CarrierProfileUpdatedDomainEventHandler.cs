using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierProfileUpdatedDomainEventHandler(INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<CarrierProfileUpdatedDomainEvent>
    {
        public async Task Handle(CarrierProfileUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var notificationCreated = Notification.Create(notification.Id, "Profile Updated", "Your carrier profile has been updated successfully.");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
