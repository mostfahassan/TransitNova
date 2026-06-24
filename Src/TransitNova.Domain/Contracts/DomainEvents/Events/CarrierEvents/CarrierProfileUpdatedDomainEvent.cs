
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierProfileUpdatedDomainEvent(Guid Id) : IDomainEvent;

}
