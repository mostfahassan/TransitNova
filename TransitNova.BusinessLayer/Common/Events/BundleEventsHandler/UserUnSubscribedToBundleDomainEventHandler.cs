using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;

namespace TransitNova.BusinessLayer.Common.Events.BundleEventsHandler
{
    public class UserUnSubscribedToBundleDomainEventHandler
        : INotificationHandler<UserUnSubscribedToBundleDomainEvent>
    {
        public Task Handle(UserUnSubscribedToBundleDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}