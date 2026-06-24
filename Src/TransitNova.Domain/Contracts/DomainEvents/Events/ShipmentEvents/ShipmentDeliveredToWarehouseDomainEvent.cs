
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentDeliveredToWarehouseDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;

}
