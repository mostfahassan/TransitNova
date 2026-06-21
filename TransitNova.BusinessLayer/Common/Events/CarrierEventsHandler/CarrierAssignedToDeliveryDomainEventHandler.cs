using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierAssignedToDeliveryDomainEventHandler
        : INotificationHandler<CarrierAssignedToDeliveryDomainEvent>
    {
        public Task Handle(CarrierAssignedToDeliveryDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}