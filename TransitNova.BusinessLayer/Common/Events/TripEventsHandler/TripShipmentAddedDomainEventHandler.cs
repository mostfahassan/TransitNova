using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripShipmentAddedDomainEventHandler
        : INotificationHandler<TripShipmentAddedDomainEvent>
    {
        public Task Handle(TripShipmentAddedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}