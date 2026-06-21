using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentUpdatedDomainEventHandler
        : INotificationHandler<ShipmentUpdatedDomainEvent>
    {
        public async Task Handle(
            ShipmentUpdatedDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}