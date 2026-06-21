using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierAssignedToPickupDomainEventHandler
        : INotificationHandler<CarrierAssignedToPickupDomainEvent>
    {
        public Task Handle(CarrierAssignedToPickupDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}