
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierCreatedDomainEvent(Guid Id, string Code) : IDomainEvent;
    public sealed record CarrierAdditionalInfoAddedDomainEvent(Guid Id, string LicenseNumber, int MaxDailyShipments) : IDomainEvent;
    public sealed record CarrierProfileUpdatedDomainEvent(Guid Id) : IDomainEvent;
    public sealed record CarrierAssignedToPickupDomainEvent(Guid Id, int AssignedShipmentsCount, CarrierStatus Status) : IDomainEvent;
    public sealed record CarrierAssignedToDeliveryDomainEvent(Guid Id, int AssignedShipmentsCount, CarrierStatus Status) : IDomainEvent;
    public sealed record CarrierTripStartedDomainEvent(Guid Id, CarrierStatus Status) : IDomainEvent;
    public sealed record CarrierShipmentCompletedDomainEvent(Guid Id, int AssignedShipmentsCount, decimal RemainingShipmentsCount, int CompletedShipmentCount) : IDomainEvent;
    public sealed record CarrierRatedDomainEvent(Guid Id, int Rating, decimal AverageRating, int TotalRatings) : IDomainEvent;
    public sealed record CarrierDeletedDomainEvent(Guid Id) : IDomainEvent;
}

