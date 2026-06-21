using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentDeletedDomainEventHandler
        : INotificationHandler<ShipmentDeletedDomainEvent>
    {
        public async Task Handle(
            ShipmentDeletedDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}