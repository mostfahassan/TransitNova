using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public sealed record SubscribeToBundleCommand(Guid RequestId, Guid UserId,Guid BundleId)
        : IdempotentCommand<BaseResult>(RequestId);
}
