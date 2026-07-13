using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentRejectedDomainEventHandler(
        IShipmentQueryRepository shipmentRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentRejectedDomainEvent>
    {
        public async Task Handle(
            ShipmentRejectedDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var shipment = await shipmentRepository.GetShipmentForCommandsAsync(notification.Id, cancellationToken);
            if (shipment is null)
                return;

            var notificationCreated = Notification.Create(
                shipment.Sender.AppUserId,
                "Shipment Rejected",
                $"Shipment {notification.TrackingNumber} was rejected: {notification.RejectionReason}");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
