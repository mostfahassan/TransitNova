
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentCreatedDomainEvent(Guid SenderId , Guid Id, string TrackingNumber) : IDomainEvent;

}
