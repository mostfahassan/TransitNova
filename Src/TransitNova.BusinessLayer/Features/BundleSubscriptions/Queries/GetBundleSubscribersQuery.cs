using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries
{
    public sealed record GetBundleSubscribersQuery(Guid BundleId)
        : IQuery<Result<List<BundleSubscriptionDetailsDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Admins.Subscribers(BundleId);
    }   
}
