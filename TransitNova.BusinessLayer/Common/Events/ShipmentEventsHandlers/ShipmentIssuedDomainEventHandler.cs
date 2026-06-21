using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentIssuedDomainEventHandler
        : INotificationHandler<ShipmentIssuedDomainEvent>
    {
        public async Task Handle(
            ShipmentIssuedDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}