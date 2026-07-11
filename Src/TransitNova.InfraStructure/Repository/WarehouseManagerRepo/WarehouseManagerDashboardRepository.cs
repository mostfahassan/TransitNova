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

    
        public async Task<WarehouseManagerKpiDto> GetWarehouseStatsAsync(Guid warehouseId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Warehouses
                .AsNoTracking()
                .Where(w => w.Id == warehouseId)
                .Select(w => new WarehouseManagerKpiDto
                {
                    TotalCarriers = context.Carriers.Count(c => c.HomeWarehouseId == warehouseId),
                    ActiveCarriers = context.Carriers.Count(c => c.HomeWarehouseId == warehouseId && c.CurrentState),
                    TotalShipments = context.Shipments.Count(s => s.Trip != null && s.Trip.WarehouseId == warehouseId),
                    DeliveredShipments = context.Shipments.Count(s => s.Trip != null && s.Trip.WarehouseId == warehouseId && s.CurrentStatus == ShipmentStatuses.Delivered),
                    InTransitShipments = context.Shipments.Count(s => s.Trip != null && s.Trip.WarehouseId == warehouseId && s.CurrentStatus == ShipmentStatuses.InTransit),
                    TotalTrips = context.Trips.Count(t => t.WarehouseId == warehouseId),
                    ActiveTrips = context.Trips.Count(t => t.WarehouseId == warehouseId && t.Status == TripStatus.Active),
                    CompletedTrips = context.Trips.Count(t => t.WarehouseId == warehouseId && t.Status == TripStatus.Completed)

                })
                .FirstOrDefaultAsync(cancellationToken) ?? new WarehouseManagerKpiDto();
        }
    }
}
