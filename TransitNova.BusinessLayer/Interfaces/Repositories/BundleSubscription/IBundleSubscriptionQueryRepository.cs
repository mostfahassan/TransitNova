
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.UserProfile;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscription
{
    public interface IBundleSubscriptionQueryRepository
    {
        Task<BundleSubscriptionDetailsDto?> GetSubscriptionDetails(Guid subscriptionId, CancellationToken ct);
        Task<UserProfileDto?> GetSubscribedUser(Guid userId,Guid bundleId, CancellationToken ct);
        Task<IEnumerable<UserProfileDto>> GetSubscribedUsers(Guid bundleId, CancellationToken ct);
    }
}
