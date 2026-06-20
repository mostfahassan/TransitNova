using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Admin
{
    internal class AdminStatisticsQueryRepository(IDbContextFactory<AppDbContext> contextFactory) : IAdminStatisticsQueryRepository
    {
        
        public async Task<int> GetTotalCarriersAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Carriers.AsNoTracking().CountAsync(cancellationToken);

        }
        public async Task<int> GetTotalOperationManagersCountAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.OperationManagerProfiles.AsNoTracking()
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetTotalShipmentsCountAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Shipments.AsNoTracking().CountAsync(cancellationToken);
        }
        public async Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.UserProfiles.AsNoTracking().CountAsync(cancellationToken);
        }
        public async Task<int> GetTotalActiveTripsAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Trips.AsNoTracking().CountAsync(t => t.Status == TripStatus.Active || t.Status == TripStatus.Planned,cancellationToken);
        }
    }
}
