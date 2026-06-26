using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles
{
    public sealed record UnsubscribeFromBundleCommand(Guid RequestId, Guid UserId, Guid BundleId)
        : IdempotentCommand<BaseResult>(RequestId);
}
