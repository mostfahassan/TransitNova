using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;

namespace TransitNova.BusinessLayer.Features.Zones.Queries
{
    public sealed record GetZonesByCityQuery(int CityId, ZoneFilterDto Filter) : IQuery<Result<PagedResult<ZoneDto>>>;
}

