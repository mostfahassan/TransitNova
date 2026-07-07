using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Trips.Queries
{
    public sealed record GetTripDetailsQuery(Guid TripId, Guid? HandlerId = null) : IQuery<Result<TripDetailsDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Trips.Details(TripId, HandlerId);
    }
}