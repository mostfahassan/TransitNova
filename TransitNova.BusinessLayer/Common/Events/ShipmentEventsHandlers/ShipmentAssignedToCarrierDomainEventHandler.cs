using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentAssignedToCarrierDomainEventHandler(
        IShipmentQueryRepository shipmentRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentAssignedToCarrierDomainEvent>
    {
        public async Task Handle(
            ShipmentAssignedToCarrierDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var shipment = await shipmentRepository.GetShipmentForCommands(notification.Id, cancellationToken);
            if (shipment is null)
                return;

            var carrierId = shipment.ShipmentStates
                .FirstOrDefault(status => status.CurrentState && status.StatusType == notification.Status)
                ?.CarrierId;

            if (carrierId.HasValue)
            {
                var carrierNotification = Notification.Create(
                    carrierId.Value,
                    "Shipment Assigned",
                    $"Shipment {notification.TrackingNumber} has been assigned to you.");
                await notificationRepo.AddNotificationAsync(carrierNotification, cancellationToken);
            }

            var senderNotification = Notification.Create(
                shipment.SenderId,
                "Carrier Assigned",
                $"A carrier has been assigned to shipment {notification.TrackingNumber}.");
            await notificationRepo.AddNotificationAsync(senderNotification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
