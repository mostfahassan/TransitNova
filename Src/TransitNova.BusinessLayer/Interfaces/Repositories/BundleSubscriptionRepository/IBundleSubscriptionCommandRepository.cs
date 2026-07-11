using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;

public interface IBundleSubscriptionCommandRepository
{
    Task<Bundle?> GetBundleForSubscriptionAsync(Guid bundleId, CancellationToken cancellationToken);
}
