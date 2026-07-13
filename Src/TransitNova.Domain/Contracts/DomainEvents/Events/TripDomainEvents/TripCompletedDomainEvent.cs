using TransitNova.Domain.Enums.Trip;

namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripCompletedDomainEvent(Guid Id, TripStatus Status, DateTime EndTime, int TotalShipments) : IDomainEvent;
}