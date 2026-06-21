using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentCancelledDomainEventHandler
        : INotificationHandler<ShipmentCancelledDomainEvent>
    {
        public async Task Handle(
            ShipmentCancelledDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}