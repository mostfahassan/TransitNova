using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.BundleSubscriptions;

internal sealed class BundleSubscriptionCommandRepository(AppDbContext context) : IBundleSubscriptionCommandRepository
{
    public Task<Bundle?> GetBundleForSubscriptionAsync(Guid bundleId, CancellationToken cancellationToken)
    {
        return context.Bundles
            .Include(bundle => bundle.Subscriptions)
            .FirstOrDefaultAsync(bundle => bundle.Id == bundleId, cancellationToken);
    }
}
