using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Zones.Queries
{
    public sealed record FilterZonesQuery(ZoneFilterDto Filter) : IQuery<Result<PagedResult<ZoneDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Zones.Filter(Filter);
    }
}


