using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripShipmentAddedDomainEventHandler(ITripQueryRepository tripRepository, INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<TripShipmentAddedDomainEvent>
    {
        public async Task Handle(TripShipmentAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripAsync(notification.Id, cancellationToken);
            if (trip is null) return;
            var recipientIds = trip.Shipments.Select(shipment => shipment.SenderId).Append(trip.CarrierId).Distinct();
            foreach (var recipientId in recipientIds)
            {
                var notificationCreated = Notification.Create(recipientId, "Shipment Added to Trip", $"A shipment was added to the trip. Total shipments: {notification.TotalShipments}.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
