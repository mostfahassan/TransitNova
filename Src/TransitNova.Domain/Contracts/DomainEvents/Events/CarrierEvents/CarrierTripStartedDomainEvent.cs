
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierTripStartedDomainEvent(Guid AppUserId, CarrierStatus Status) : IDomainEvent;

}
