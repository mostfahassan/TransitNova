using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierDeletedDomainEventHandler(INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<CarrierDeletedDomainEvent>
    {
        public async Task Handle(CarrierDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var notificationCreated = Notification.Create(notification.Id, "Carrier Account Removed", "Your carrier profile has been removed.");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
