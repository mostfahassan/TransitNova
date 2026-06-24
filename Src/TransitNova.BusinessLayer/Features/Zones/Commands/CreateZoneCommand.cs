using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;

namespace TransitNova.BusinessLayer.Features.Zones.Commands
{
    public sealed record CreateZoneCommand(Guid RequestId, CreateZoneDto Dto) : IdempotentCommand<Result<ZoneDto>>(RequestId);
}
