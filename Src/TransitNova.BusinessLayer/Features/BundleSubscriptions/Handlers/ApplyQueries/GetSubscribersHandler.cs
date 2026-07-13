using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.Features.BundleSubscriptions.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;

namespace TransitNova.BusinessLayer.Features.BundleSubscriptions.Handlers.ApplyQueries;

public sealed class GetSubscribersHandler(
    IBundleSubscriptionQueryRepository subscriptionRepository,
    ILogger<GetSubscribersHandler> logger)
    : IQueryHandler<GetSubscribersQuery, Result<List<BundleSubscriptionDetailsDto>>>
{
    public async Task<Result<List<BundleSubscriptionDetailsDto>>> Handle(GetSubscribersQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all bundle subscribers.");

        var subscribers = (await subscriptionRepository.GetSubscribersAsync(ct)).ToList();
        logger.LogInformation("Retrieved {Count} bundle subscribers.", subscribers.Count);

        return Result<List<BundleSubscriptionDetailsDto>>.Success(subscribers);
    }
}