using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Commands
{
    public sealed record DeleteCountryCommand(Guid RequestId, int Id) : IdempotentCommand<BaseResult>(RequestId);
}
