using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Queries
{
    public sealed record GetTripDetailsQuery(Guid TripId) : IQuery<Result<TripDetailsDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Trips.Details(TripId);
    }
}

