using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Handlers.ApplyQueries
{
    public sealed class GetBundleSubscribersHandler(
        IBundleSubscriptionQueryRepository subscriptionRepository,
        ILogger<GetBundleSubscribersHandler> logger)
        : IQueryHandler<GetBundleSubscribersQuery, Result<List<BundleSubscriptionDetailsDto>>>
    {
        public async Task<Result<List<BundleSubscriptionDetailsDto>>> Handle(GetBundleSubscribersQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving active subscribers for Bundle {BundleId}", request.BundleId);

            var subscribers = (await subscriptionRepository.GetBundleSubscribersAsync(request.BundleId, ct)).ToList();
            if (subscribers.Count == 0)
            {
                logger.LogInformation("No active subscribers found for Bundle {BundleId}", request.BundleId);
                return Result<List<BundleSubscriptionDetailsDto>>.Success([]);
            }

            logger.LogInformation("Retrieved {Count} active subscribers for Bundle {BundleId}", subscribers.Count, request.BundleId);
            return Result<List<BundleSubscriptionDetailsDto>>.Success(subscribers);
        }
    }
}
