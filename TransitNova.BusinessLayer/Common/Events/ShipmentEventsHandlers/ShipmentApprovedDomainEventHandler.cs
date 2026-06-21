using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentApprovedDomainEventHandler
        : INotificationHandler<ShipmentApprovedDomainEvent>
    {
        public async Task Handle(
            ShipmentApprovedDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}