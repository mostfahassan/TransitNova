using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Trips
{
    public sealed record FilterTripsQuery(FilterTripsDto Filter)
        : IQuery<Result<PagedResult<TripDetailsDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Trips.Filter(Filter);
    }
}
