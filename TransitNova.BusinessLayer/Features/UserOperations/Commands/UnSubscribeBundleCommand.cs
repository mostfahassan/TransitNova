using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public sealed record UnSubscribeBundleCommand(Guid RequestId, Guid UserId, int BundleId)
        : IdempotantCommand<BaseResult>(RequestId);
}
