using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;

namespace TransitNova.BusinessLayer.Features.Zones.Queries
{
    public sealed record GetZoneByIdQuery(Guid Id) : IQuery<Result<ZoneDto?>>;
}

