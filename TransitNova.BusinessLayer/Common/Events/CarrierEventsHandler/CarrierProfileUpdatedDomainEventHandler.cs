using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierProfileUpdatedDomainEventHandler
        : INotificationHandler<CarrierProfileUpdatedDomainEvent>
    {
        public Task Handle(CarrierProfileUpdatedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}