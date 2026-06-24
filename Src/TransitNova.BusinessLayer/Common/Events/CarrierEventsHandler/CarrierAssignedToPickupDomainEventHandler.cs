using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierAssignedToPickupDomainEventHandler(INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<CarrierAssignedToPickupDomainEvent>
    {
        public async Task Handle(CarrierAssignedToPickupDomainEvent notification, CancellationToken cancellationToken)
        {
            var notificationCreated = Notification.Create(notification.Id, "Pickup Assigned", $"A pickup shipment has been assigned to you. Active assignments: {notification.AssignedShipmentsCount}.");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
