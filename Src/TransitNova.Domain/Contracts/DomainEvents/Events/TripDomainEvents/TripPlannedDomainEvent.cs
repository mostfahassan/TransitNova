using TransitNova.Domain.Enums.Trip;

namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripPlannedDomainEvent(Guid Id, Guid WarehouseId, TripType TripType, int TotalShipments) : IDomainEvent;
}