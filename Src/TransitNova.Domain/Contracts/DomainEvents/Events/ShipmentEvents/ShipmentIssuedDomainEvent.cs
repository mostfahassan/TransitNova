
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentIssuedDomainEvent(Guid Id, string TrackingNumber,string IssueMessage) : IDomainEvent;

}
