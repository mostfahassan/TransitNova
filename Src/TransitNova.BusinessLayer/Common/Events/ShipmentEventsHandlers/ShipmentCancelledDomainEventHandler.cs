using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentCancelledDomainEventHandler(
        IOperationManagerQueryRepository operationManager,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentCancelledDomainEvent>
    {
        public async Task Handle(
            ShipmentCancelledDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var operationManagerIds = await operationManager.GetOperationManagersIdsAsync(cancellationToken);

            foreach (var operationManagerId in operationManagerIds)
            {
                var notificationCreated = Notification.Create(
                    operationManagerId,
                    "Shipment Cancelled",
                    $"Shipment {notification.TrackingNumber} has been cancelled.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
