
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierAssignedToPickupDomainEvent(Guid AppUserId, int AssignedShipmentsCount, CarrierStatus Status) : IDomainEvent;

}
