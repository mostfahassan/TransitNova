using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.Bundles.Commands
{
    public sealed record DeleteBundleCommand(Guid RequestId, Guid Id)
        : IdempotentCommand<BaseResult>(RequestId);
}
