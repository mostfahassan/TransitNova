
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentCreatedDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentCancelledDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentApprovedDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentRejectedDomainEvent(Guid Id, string TrackingNumber,string RejectedReaseon) : IDomainEvent;
    public sealed record ShipmentUpdatedDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentDeletedDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentIssuedDomainEvent(Guid Id, string TrackingNumber,string IssueMessage) : IDomainEvent;
    public sealed record ShipmentDeliveredToWarehouseDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentDeliveredDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
    public sealed record ShipmentAssignedToCarrierDomainEvent(Guid Id, string TrackingNumber,ShipmentStatuses Status) : IDomainEvent;
}
