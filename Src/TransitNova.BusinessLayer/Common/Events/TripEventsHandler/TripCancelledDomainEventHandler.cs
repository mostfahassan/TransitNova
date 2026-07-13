using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripCancelledDomainEventHandler(
        ITripQueryRepository tripRepository,
        ICarrierQueryRepository carrierRepository,
        IUserQueryRepository userRepository,
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork)
        : INotificationHandler<TripCancelledDomainEvent>
    {
        public async Task Handle(TripCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripAsync(t => t.Id == notification.Id, cancellationToken);
            if (trip is null)
                return;

            var recipientIds = new HashSet<Guid>();
            var carrier = await carrierRepository.GetCarrierAsync(c => c.Id == trip.CarrierId, cancellationToken);
            if (carrier is not null)
                recipientIds.Add(carrier.AppUserId);

            foreach (var shipment in trip.Shipments)
            {
                var senderAppUserId = await userRepository.GetAppUserIdByProfileIdAsync(shipment.SenderId, cancellationToken);
                if (senderAppUserId.HasValue)
                    recipientIds.Add(senderAppUserId.Value);
            }

            foreach (var recipientId in recipientIds)
            {
                var notificationCreated = Notification.Create(recipientId, "Trip Cancelled", "The assigned trip has been cancelled.");
                await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}