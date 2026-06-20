
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.Admin;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Admin
{
    internal class AdminOperationalHealth(IDbContextFactory<AppDbContext> contextFactory) : IAdminOperationalHealth
    {
        public async Task<int> ActiveOperationManagersAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.OperationManagerProfiles.AsNoTracking().CountAsync(op => op.CurrentState, cancellationToken);
        }

        public async Task<decimal> AverageCarrierRatingAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers.AsNoTracking().AverageAsync(r => r.AverageRating,cancellationToken);
            
        }


        public async Task<int> BusyCarriersAsync(CancellationToken cancellationToken)
        {
            CarrierStatus[] busyStatus = [CarrierStatus.AssignedToDeliveryShipment, CarrierStatus.OnTrip, CarrierStatus.AssignedToPickUpShipment];
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers.AsNoTracking()
                 .CountAsync(c => busyStatus.Contains(c.Status), cancellationToken);
        }
        public async Task<int> ActiveShipmentAsync(CancellationToken cancellationToken)
        {
            ShipmentStatuses[] ActiveStatuses = [ShipmentStatuses.OutForPickup, ShipmentStatuses.OutForDelivery, ShipmentStatuses.InTransit];
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Shipments.AsNoTracking()
                 .CountAsync(c => ActiveStatuses.Contains(c.CurrentStatus), cancellationToken);
        }

        public async Task<decimal> CancelledShipmentRateAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var stats = await context.Shipments
                        .AsNoTracking()
                        .GroupBy(_ => 1)
                        .Select(g => new
                        {
                            Total = g.Count(),
                            Cancelled = g.Count(sh => sh.CurrentStatus == ShipmentStatuses.Cancelled)
                        })
                        .FirstOrDefaultAsync(cancellationToken);

            return stats is null|| stats.Total == 0  
                            ? 0
                            : stats.Total / stats.Cancelled * 100;
        }

        public async Task<decimal> DeliverySuccessRateAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers.AverageAsync(c => c.SuccessRate, cancellationToken);
        }

        public async Task<int> GetActiveCarriersCountAsync(CancellationToken cancellationToken)
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers.CountAsync(c => c.CurrentState, cancellationToken);
        }

        public  async Task<int> AvailableCarriersAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers.CountAsync(c => c.Status == CarrierStatus.Available, cancellationToken);
        }
    }
}
