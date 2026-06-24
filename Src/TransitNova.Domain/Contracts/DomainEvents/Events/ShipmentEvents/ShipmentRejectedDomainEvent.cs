
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentRejectedDomainEvent(Guid Id, string TrackingNumber,string RejectionReason) : IDomainEvent;

}
