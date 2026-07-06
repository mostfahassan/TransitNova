namespace TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents
{
    public sealed record ShipmentPickedUpDomainEvent(Guid Id, string TrackingNumber) : IDomainEvent;
}
