
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierCreatedDomainEvent(Guid AppUserId,string FullName,string Phone ,string Code) : IDomainEvent;

}
