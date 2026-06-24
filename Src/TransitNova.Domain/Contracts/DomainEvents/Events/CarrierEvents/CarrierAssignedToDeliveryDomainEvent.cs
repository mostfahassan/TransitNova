
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierAssignedToDeliveryDomainEvent(Guid Id, int AssignedShipmentsCount, CarrierStatus Status) : IDomainEvent;

}
