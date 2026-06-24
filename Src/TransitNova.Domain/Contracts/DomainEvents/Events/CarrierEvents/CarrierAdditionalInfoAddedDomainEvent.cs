
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierAdditionalInfoAddedDomainEvent(Guid Id, string LicenseNumber, int MaxDailyShipments) : IDomainEvent;

}
