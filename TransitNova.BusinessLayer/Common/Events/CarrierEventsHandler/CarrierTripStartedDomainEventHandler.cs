using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierTripStartedDomainEventHandler
        : INotificationHandler<CarrierTripStartedDomainEvent>
    {
        public Task Handle(CarrierTripStartedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}