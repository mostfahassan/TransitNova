namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentCreatedDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
}