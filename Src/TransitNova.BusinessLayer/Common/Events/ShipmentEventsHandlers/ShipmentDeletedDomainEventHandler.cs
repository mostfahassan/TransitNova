using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentDeletedDomainEventHandler(
        IOperationManagerQueryRepository operationManager,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentDeletedDomainEvent>
    {
        public async Task Handle(
            ShipmentDeletedDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var operationManagerIds = await operationManager.GetOperationManagersIdsAsync(cancellationToken);

            foreach (var operationManagerId in operationManagerIds)
            {
                var notificationCreated = Notification.Create(
                    operationManagerId,
                    "Shipment Deleted",
                    $"Shipment {notification.TrackingNumber} has been deleted.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
