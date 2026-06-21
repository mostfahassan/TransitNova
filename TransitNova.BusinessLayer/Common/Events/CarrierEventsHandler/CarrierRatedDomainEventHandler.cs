using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierRatedDomainEventHandler
        : INotificationHandler<CarrierRatedDomainEvent>
    {
        public Task Handle(CarrierRatedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}