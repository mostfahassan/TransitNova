using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;

namespace TransitNova.BusinessLayer.Features.Zones.Commands
{
    public sealed record UpdateZoneCommand(Guid RequestId, UpdateZoneDto Dto) : IdempotantCommand<BaseResult>(RequestId);
}
