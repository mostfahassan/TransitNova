using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentPickedUpDomainEventHandler(
        IShipmentQueryRepository shipmentRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentPickedUpDomainEvent>
    {
        public async Task Handle(
            ShipmentPickedUpDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var shipment = await shipmentRepository.GetShipmentForCommandsAsync(notification.Id, cancellationToken);
            if (shipment is null)
                return;

            var carrierAppUserId = shipment.ShipmentStates
                .FirstOrDefault(status => status.CurrentState && status.StatusType == ShipmentStatuses.PickedUp)
                ?.Carrier?.AppUserId;

            if (carrierAppUserId.HasValue)
            {
                var carrierNotification = Notification.Create(
                    carrierAppUserId.Value,
                    "Shipment Picked Up",
                    $"Shipment {notification.TrackingNumber} pickup has been confirmed.");
                await notificationRepo.AddNotificationAsync(carrierNotification, cancellationToken);
            }

            var senderNotification = Notification.Create(
                shipment.Sender.AppUserId,
                "Shipment Picked Up",
                $"Shipment {notification.TrackingNumber} has been picked up by the assigned carrier.");
            await notificationRepo.AddNotificationAsync(senderNotification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
