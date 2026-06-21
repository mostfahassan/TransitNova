using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;

namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripCompletedDomainEventHandler
        : INotificationHandler<TripCompletedDomainEvent>
    {
        public Task Handle(TripCompletedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}