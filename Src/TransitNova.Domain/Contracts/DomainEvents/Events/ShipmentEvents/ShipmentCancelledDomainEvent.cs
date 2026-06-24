
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentCancelledDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;

}
