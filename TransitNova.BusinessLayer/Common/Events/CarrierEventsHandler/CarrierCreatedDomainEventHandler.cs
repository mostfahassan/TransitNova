using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierCreatedDomainEventHandler
        : INotificationHandler<CarrierCreatedDomainEvent>
    {
        public Task Handle(CarrierCreatedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}