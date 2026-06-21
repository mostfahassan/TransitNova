using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierShipmentCompletedDomainEventHandler
        : INotificationHandler<CarrierShipmentCompletedDomainEvent>
    {
        public Task Handle(CarrierShipmentCompletedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}