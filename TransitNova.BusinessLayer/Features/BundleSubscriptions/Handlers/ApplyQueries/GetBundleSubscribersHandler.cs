using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscription;

namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Handlers.ApplyQueries
{
    public sealed class GetBundleSubscribersHandler(
        IBundleSubscriptionQueryRepository subscriptionRepository,
        ILogger<GetBundleSubscribersHandler> logger)
        : IQueryHandler<GetBundleSubscribersQuery, Result<List<UserProfileDto>>>
    {
        public async Task<Result<List<UserProfileDto>>> Handle(GetBundleSubscribersQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving active subscribers for Bundle {BundleId}", request.BundleId);

            var subscribers = (await subscriptionRepository.GetSubscribedUsers(request.BundleId, ct)).ToList();
            if (subscribers.Count == 0)
            {
                logger.LogInformation("No active subscribers found for Bundle {BundleId}", request.BundleId);
                return Result<List<UserProfileDto>>.Success([]);
            }

            logger.LogInformation("Retrieved {Count} active subscribers for Bundle {BundleId}", subscribers.Count, request.BundleId);
            return Result<List<UserProfileDto>>.Success(subscribers);
        }
    }
}
