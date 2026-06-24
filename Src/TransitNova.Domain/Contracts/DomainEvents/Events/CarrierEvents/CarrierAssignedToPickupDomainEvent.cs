
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierAssignedToPickupDomainEvent(Guid Id, int AssignedShipmentsCount, CarrierStatus Status) : IDomainEvent;

}
