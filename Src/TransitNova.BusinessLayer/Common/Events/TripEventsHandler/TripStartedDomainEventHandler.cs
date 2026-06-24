using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripStartedDomainEventHandler(ITripQueryRepository tripRepository, INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<TripStartedDomainEvent>
    {
        public async Task Handle(TripStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripAsync(notification.Id, cancellationToken);
            if (trip is null) return;
            var recipientIds = trip.Shipments.Select(shipment => shipment.SenderId).Append(trip.CarrierId).Distinct();
            foreach (var recipientId in recipientIds)
            {
                var notificationCreated = Notification.Create(recipientId, "Trip Started", $"The {notification.TripType} trip has started.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
