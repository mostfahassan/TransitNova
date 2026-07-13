using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentUpdatedDomainEventHandler(
        IOperationManagerQueryRepository operationManager,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentUpdatedDomainEvent>
    {
        public async Task Handle(
            ShipmentUpdatedDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var operationManagerIds = await operationManager.GetOperationManagersAppUserIdsAsync(cancellationToken);

            foreach (var operationManagerId in operationManagerIds)
            {
                var notificationCreated = Notification.Create(
                    operationManagerId,
                    "Shipment Updated",
                    $"Shipment {notification.TrackingNumber} has been updated.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
