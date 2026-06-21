using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;
namespace TransitNova.BusinessLayer.Common.Events.TripEventsHandler
{
    public class TripPlannedDomainEventHandler
        : INotificationHandler<TripPlannedDomainEvent>
    {
        public Task Handle(TripPlannedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}