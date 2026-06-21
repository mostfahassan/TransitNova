using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
namespace TransitNova.BusinessLayer.Common.Events.ShipmentEventsHandlers
{
    public class ShipmentCreatedDomainEventHandler
        : INotificationHandler<ShipmentCreatedDomainEvent>
    {
        public async Task Handle(
            ShipmentCreatedDomainEvent notification,
            CancellationToken cancellationToken)
        {
        }
    }
}