using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.UserProfile;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository
{
    public interface IBundleSubscriptionQueryRepository
    {
        Task<BundleSubscriptionDetailsDto?> GetSubscriptionDetailsAsync(Guid subscriptionId, CancellationToken ct);
        Task<UserProfileDto?> GetSubscribedUserAsync(Guid userId,Guid bundleId, CancellationToken ct);
        Task<IEnumerable<UserProfileDto>> GetSubscribedUsersAsync(Guid bundleId, CancellationToken ct);
    }
}
