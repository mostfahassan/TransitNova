using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Queries
{
    public sealed record FilterCitiesQuery(CityFilterDto Filter) : IQuery<Result<PagedResult<CityDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Cities.Filter(Filter);
    }
}

