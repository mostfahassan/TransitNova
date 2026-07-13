
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents
{
    public sealed record CarrierRatedDomainEvent(Guid AppUserId, int Rating, decimal AverageRating, int TotalRatings) : IDomainEvent;

}
