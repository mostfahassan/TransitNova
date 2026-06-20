using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Commands
{
    public sealed record CreateGovernmentCommand(Guid RequestId, string Name, int CountryId)
        : IdempotantCommand<Result<GovernmentDto>>(RequestId);
}
