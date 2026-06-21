using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripUpdatedDomainEventHandler
        : INotificationHandler<TripUpdatedDomainEvent>
    {
        public Task Handle(TripUpdatedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}