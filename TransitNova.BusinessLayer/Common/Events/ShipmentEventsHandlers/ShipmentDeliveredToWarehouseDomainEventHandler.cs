using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentDeliveredToWarehouseDomainEventHandler
        : INotificationHandler<ShipmentDeliveredToWarehouseDomainEvent>
    {
        public async Task Handle(
            ShipmentDeliveredToWarehouseDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}