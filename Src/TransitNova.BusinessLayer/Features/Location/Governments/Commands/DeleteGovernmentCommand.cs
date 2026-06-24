using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Commands
{
    public sealed record DeleteGovernmentCommand(Guid RequestId, int Id) : IdempotentCommand<BaseResult>(RequestId);
}
