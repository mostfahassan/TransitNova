
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierTripStartedDomainEvent(Guid Id, CarrierStatus Status) : IDomainEvent;

}
