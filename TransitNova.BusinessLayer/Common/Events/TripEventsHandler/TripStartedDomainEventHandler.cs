using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripStartedDomainEventHandler
        : INotificationHandler<TripStartedDomainEvent>
    {
        public Task Handle(TripStartedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}