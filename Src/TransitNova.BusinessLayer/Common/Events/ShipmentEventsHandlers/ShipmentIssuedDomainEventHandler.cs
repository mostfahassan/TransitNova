using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentIssuedDomainEventHandler(
        IOperationManagerQueryRepository operationManager,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<ShipmentIssuedDomainEvent>
    {
        public async Task Handle(
            ShipmentIssuedDomainEvent notification,
            CancellationToken cancellationToken)
        {
            var operationManagerIds = await operationManager.GetOperationManagersAppUserIdsAsync(cancellationToken);

            foreach (var operationManagerId in operationManagerIds)
            {
                var notificationCreated = Notification.Create(
                    operationManagerId,
                    "Shipment Issue Reported",
                    $"Shipment {notification.TrackingNumber} has a reported issue: {notification.IssueMessage}");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
