using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripPlannedDomainEventHandler(
        ITripQueryRepository tripRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<TripPlannedDomainEvent>
    {
        public async Task Handle(TripPlannedDomainEvent notification, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripAsync(t => t.Id == notification.Id, cancellationToken);
            if (trip is null)
                return;

            var recipientIds = trip.Shipments.Select(shipment => shipment.SenderId)
                .Append(trip.CarrierId)
                .Distinct();

            foreach (var recipientId in recipientIds)
            {
                var notificationCreated = Notification.Create(
                    recipientId,
                    "Trip Planned",
                    $"A {notification.TripType} trip has been planned with {notification.TotalShipments} shipment(s).");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
