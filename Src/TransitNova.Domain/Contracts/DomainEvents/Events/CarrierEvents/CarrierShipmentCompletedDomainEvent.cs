
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierShipmentCompletedDomainEvent(Guid AppUserId, int AssignedShipmentsCount, decimal RemainingShipmentsCount, int CompletedShipmentCount) : IDomainEvent;

}
