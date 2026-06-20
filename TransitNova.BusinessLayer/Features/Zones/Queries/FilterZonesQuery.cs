using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Zones.Queries
{
    public sealed record FilterZonesQuery(ZoneFilterDto Filter) : IQuery<Result<PagedResult<ZoneDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.FilterZones(Filter);
    }
}

