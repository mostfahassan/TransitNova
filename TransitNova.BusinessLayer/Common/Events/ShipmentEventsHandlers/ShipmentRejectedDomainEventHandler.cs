using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentRejectedDomainEventHandler
        : INotificationHandler<ShipmentRejectedDomainEvent>
    {
        public async Task Handle(
            ShipmentRejectedDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}