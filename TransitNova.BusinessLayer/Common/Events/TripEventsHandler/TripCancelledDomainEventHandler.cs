using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripCancelledDomainEventHandler
        : INotificationHandler<TripCancelledDomainEvent>
    {
        public Task Handle(TripCancelledDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}