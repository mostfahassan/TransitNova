using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Zones.Commands
{
    public sealed record DeleteZoneCommand(Guid RequestId, Guid Id) : IdempotantCommand<BaseResult>(RequestId);
}
