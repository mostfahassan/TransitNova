using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;
namespace TransitNova.BusinessLayer.Features.Bundles.Commands
{
    // --- Features/Bundle/Commands/UpdateBundleCommand.cs ---
    public sealed record UpdateBundleCommand(Guid RequestId, UpdateBundleDto Dto, Guid AppUserId)
        : IdempotantCommand<BaseResult>(RequestId);

}
