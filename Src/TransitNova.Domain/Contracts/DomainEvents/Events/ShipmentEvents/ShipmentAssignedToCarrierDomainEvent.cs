
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentAssignedToCarrierDomainEvent(Guid Id, string TrackingNumber,ShipmentStatuses Status) : IDomainEvent;

}
