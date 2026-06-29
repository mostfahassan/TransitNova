using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;
namespace TransitNova.BusinessLayer.Features.Bundles.Commands
{
    public sealed record UpdateBundleCommand(Guid RequestId,Guid BundleId , UpdateBundleDto Dto, Guid AppUserId)
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
}


