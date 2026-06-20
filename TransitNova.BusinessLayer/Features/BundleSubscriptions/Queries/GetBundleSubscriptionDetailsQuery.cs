using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries
{
    public sealed record GetBundleSubscriptionDetailsQuery(Guid SubscriptionId)
        : IQuery<Result<BundleSubscriptionDetailsDto>>, ICachable
    {
        public string CacheKey => CacheKeys.BundleSubscriptionDetails(SubscriptionId);
    }
}
