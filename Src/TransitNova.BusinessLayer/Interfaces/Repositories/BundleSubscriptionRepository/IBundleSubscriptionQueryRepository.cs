using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.UserProfile;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository
{
    public interface IBundleSubscriptionQueryRepository
    {
        Task<BundleSubscriptionDetailsDto?> GetSubscriptionDetailsAsync(Guid subscriptionId, CancellationToken ct);
        Task<UserProfileDto?> GetSubscribedUserAsync(Guid userId, Guid bundleId, CancellationToken ct);
        Task<IEnumerable<UserProfileDto>> GetSubscribedUsersAsync(Guid bundleId, CancellationToken ct);
        Task<IEnumerable<BundleSubscriptionDetailsDto>> GetSubscribersAsync(CancellationToken ct);
        Task<IEnumerable<BundleSubscriptionDetailsDto>> GetBundleSubscribersAsync(Guid bundleId, CancellationToken ct);
        Task<ActiveBundleSubscriptionBenefitDto?> GetActiveSubscriptionForUserAsync(Guid userId, CancellationToken ct);
        Task<int> GetMonthlyAppliedBenefitCountAsync(Guid userId, Guid bundleId, DateTime monthStartUtc, DateTime monthEndUtc, CancellationToken ct);
    }
}
