using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;

namespace TransitNova.BusinessLayer.Common.Events.BundleEventsHandler
{
    public class BundleUpdatedDomainEventHandler
        : INotificationHandler<BundleUpdatedDomainEvent>
    {
        public Task Handle(BundleUpdatedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}