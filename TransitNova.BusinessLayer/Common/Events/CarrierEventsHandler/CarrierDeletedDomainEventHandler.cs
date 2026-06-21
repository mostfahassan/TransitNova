using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierDeletedDomainEventHandler
        : INotificationHandler<CarrierDeletedDomainEvent>
    {
        public Task Handle(CarrierDeletedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}