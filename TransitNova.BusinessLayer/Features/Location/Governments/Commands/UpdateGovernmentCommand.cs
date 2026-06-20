using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Commands
{
    public sealed record UpdateGovernmentCommand(Guid RequestId, int Id, string Name, int CountryId)
        : IdempotantCommand<BaseResult>(RequestId);
}
