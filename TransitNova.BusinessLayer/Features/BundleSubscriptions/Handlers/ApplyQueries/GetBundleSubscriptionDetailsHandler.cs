using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;

namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Handlers.ApplyQueries
{
    public sealed class GetBundleSubscriptionDetailsHandler(
        IBundleSubscriptionQueryRepository subscriptionRepository,
        ILogger<GetBundleSubscriptionDetailsHandler> logger)
        : IQueryHandler<GetBundleSubscriptionDetailsQuery, Result<BundleSubscriptionDetailsDto>>
    {
        public async Task<Result<BundleSubscriptionDetailsDto>> Handle(GetBundleSubscriptionDetailsQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving bundle subscription details. SubscriptionId: {SubscriptionId}", request.SubscriptionId);

            var subscription = await subscriptionRepository.GetSubscriptionDetails(request.SubscriptionId, ct);
            if (subscription is null)
            {
                logger.LogWarning("Bundle subscription not found. SubscriptionId: {SubscriptionId}", request.SubscriptionId);
                var notFoundResult = Result<BundleSubscriptionDetailsDto>.NotFound(Errors.NotFound("Bundle subscription not found."));
                return notFoundResult;
            }

            logger.LogInformation("Bundle subscription details retrieved successfully. SubscriptionId: {SubscriptionId}", request.SubscriptionId);
            var result = Result<BundleSubscriptionDetailsDto>.Success(subscription);
            return result;
        }
    }
}
