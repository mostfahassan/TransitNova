using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentAssignedToCarrierDomainEventHandler
        : INotificationHandler<ShipmentAssignedToCarrierDomainEvent>
    {
        public async Task Handle(
            ShipmentAssignedToCarrierDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}