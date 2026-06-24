
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentDeletedDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;

}
