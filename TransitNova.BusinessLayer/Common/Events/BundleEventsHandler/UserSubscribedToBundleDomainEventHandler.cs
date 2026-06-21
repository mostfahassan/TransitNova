using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
namespace TransitNova.BusinessLayer.Common.Events.BundleEventsHandler
{
    public class UserSubscribedToBundleDomainEventHandler
        : INotificationHandler<UserSubscribedToBundleDomainEvent>
    {
        public Task Handle(UserSubscribedToBundleDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
