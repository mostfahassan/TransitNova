using MediatR;
using TransitNova.BusinessLayer.Interfaces.Services.Notification;
using TransitNova.Domain.Contracts.DomainEvents.Events.NotificationEvents;
namespace TransitNova.BusinessLayer.Common.Events.NotificationEventsHandler
{
    internal sealed class NotificationCreatedDomainEventHandler(
     INotificationBroadcaster broadcaster)
     : INotificationHandler<NotificationCreatedDomainEvent>
    {
        public async Task Handle(NotificationCreatedDomainEvent notification, CancellationToken cancellationToken)
        {

            await broadcaster.SendToUserAsync(notification.UserId, notification.Title, notification.Message, cancellationToken);
        }
    }
}
