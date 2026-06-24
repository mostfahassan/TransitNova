using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentCreatedDomainEventHandler(IOperationManagerQueryRepository operationManager,
        INotificationCommand notificationRepo,
        IUnitOfWork UOW)
        : INotificationHandler<ShipmentCreatedDomainEvent>
    {
        public async Task Handle(ShipmentCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var operationManagersIds =
                await operationManager.GetOperationManagersIdsAsync(cancellationToken);

            foreach (var managerId in operationManagersIds)
            {
                var managerNotification = Notification.Create(managerId, "New Shipment Created", $"Shipment {notification.TrackingNumber} has been created.");

                await notificationRepo.AddNotificationAsync(managerNotification, cancellationToken);

            }

            var senderNotification = Notification.Create(notification.SenderId, "Shipment Created Successfully", $"Your shipment {notification.TrackingNumber} has been created successfully.");

            await notificationRepo.AddNotificationAsync(senderNotification, cancellationToken);

            await UOW.SaveChangesAsync(cancellationToken);
        }
    }
}