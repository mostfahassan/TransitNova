using TransitNova.Domain.Contracts.DomainEvents.Events.NotificationEvents;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.Domain.Entities.MainEntities
{
    public sealed class Notification : AggregateRoot<Guid>
    {
        private Notification()
        {
        }

        public Guid UserId { get; private set; }

        public string Title { get; private set; } = null!;

        public string Message { get; private set; } = null!;

        public bool IsRead { get; private set; }

        public DateTime CreatedOnUtc { get; private set; }

        public static Notification Create(
            Guid userId,
            string title,
            string message)
        {
            var notification = new Notification
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedOnUtc = DateTime.UtcNow
            };

            notification.RaiseDomainEvent(new NotificationCreatedDomainEvent(
                notification.Id,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.CreatedOnUtc,
                notification.IsRead));
            return notification;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}