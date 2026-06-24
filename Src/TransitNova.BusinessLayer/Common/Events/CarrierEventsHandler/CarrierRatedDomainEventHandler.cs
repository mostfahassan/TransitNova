using MediatR;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierRatedDomainEventHandler(INotificationCommand notificationRepo, IUnitOfWork unitOfWork)
        : INotificationHandler<CarrierRatedDomainEvent>
    {
        public async Task Handle(CarrierRatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var notificationCreated = Notification.Create(notification.Id, "New Rating Received", $"You received a {notification.Rating}-star rating. Your average rating is now {notification.AverageRating:0.0}.");
            await notificationRepo.AddNotificationAsync(notificationCreated, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
