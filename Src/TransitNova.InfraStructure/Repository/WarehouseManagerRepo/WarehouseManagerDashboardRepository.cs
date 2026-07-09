using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.WarehouseManagerRepo
{
    internal sealed class WarehouseManagerDashboardRepository(IDbContextFactory<AppDbContext> contextFactory) : IWarehouseManagerDashboardRepository
    {
        public async Task<int> ActiveCarriersAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers
                 .AsNoTracking()
                 .Where(carrier => carrier.HomeWarehouseId == warehouseId && carrier.CurrentState)
                 .CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<RecentShipmentSummary>> GetRecentShipmentSummaryAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Shipments
               .AsNoTracking()
               .Where(shipment => shipment.Trip != null && shipment.Trip.WarehouseId == warehouseId)
               .OrderByDescending(shipment => shipment.CreatedAt)
               .Take(5)
               .Select(shipment => new RecentShipmentSummary
               {
                   Id = shipment.Id,
                   TrackingNumber = shipment.TrackingNumber,
                   CurrentStatus = shipment.CurrentStatus,
                   CreatedAt = shipment.CreatedAt
               })
               .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RecentTripSummary>> GetRecentTripsSummaryAsync(Guid warehouseId ,CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Trips
             .AsNoTracking()
             .Where(trip => trip.WarehouseId == warehouseId)
              .OrderByDescending(trip => trip.PlannedDate)
                .Take(5)
                .Select(trip => new RecentTripSummary
                {
                    Id = trip.Id,
                    TripType = trip.TripType,
                    Status = trip.Status,
                    PlannedDate = trip.PlannedDate
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Shipments
                    .AsNoTracking()
                    .Where(shipment => shipment.Trip != null && shipment.Trip.WarehouseId == warehouseId)
                    .GroupBy(st => st.CurrentStatus)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);
        }
        public async Task<Dictionary<TripStatus, int>> GetTripsCountInStatusAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Trips
                    .AsNoTracking()
                    .Where(trip => trip.WarehouseId == warehouseId)
                    .GroupBy(st => st.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);
        }

        public async Task<WarehouseManagerSummary?> GetWarehouseManagerSummaryAsync(Guid managerId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.WarehouseManagersProfiles
                .AsNoTracking()
                .Where(profile => profile.Id == managerId || profile.AppUserId == managerId)
                .Select(profile => new WarehouseManagerSummary
                {
                    ManagerId = profile.Id,
                    ManagerName = profile.FullName,
                    WarehouseId = profile.Warehouse != null ? profile.Warehouse.Id : Guid.Empty,
                    WarehouseName = profile.Warehouse != null ? profile.Warehouse.Name : string.Empty,
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> TotalCarriersAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var carriers = await context.Carriers
                 .AsNoTracking()
                 .Where(carrier => carrier.HomeWarehouseId == warehouseId)
                 .CountAsync(cancellationToken);
            return carriers;
        }

        public async Task<int> TotalShipmentAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var shipments = await context.Shipments
               .AsNoTracking()
               .Where(shipment => shipment.Trip != null && shipment.Trip.WarehouseId == warehouseId)
               .CountAsync (cancellationToken);
            return shipments;
        }

        public async Task<int> TotalTripsAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var trips = await context.Trips
                .AsNoTracking()
                .Where(trip => trip.WarehouseId == warehouseId)
                .CountAsync(cancellationToken);
            return trips;
        }
    }
}
