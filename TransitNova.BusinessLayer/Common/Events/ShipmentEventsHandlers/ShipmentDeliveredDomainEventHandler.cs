using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentDeliveredDomainEventHandler
        : INotificationHandler<ShipmentDeliveredDomainEvent>
    {
        public async Task Handle(
            ShipmentDeliveredDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}