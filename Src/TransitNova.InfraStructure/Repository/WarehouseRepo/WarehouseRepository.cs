using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.WarehouseRepo
{
    internal class WarehouseRepository(AppDbContext context) : IWarehouseQueriesRepository
    {
        public async Task<List<WarehouseDto>> GetWarehousesAsync(CancellationToken ct)
            => await ProjectWarehouseDto(context.Warehouses.AsNoTracking())
                .OrderBy(w => w.Name)
                .ToListAsync(ct);

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(Guid warehouseId, CancellationToken ct)
            => await ProjectWarehouseDto(context.Warehouses.AsNoTracking().Where(w => w.Id == warehouseId))
                .FirstOrDefaultAsync(ct);

        public async Task<Warehouse?> GetWarehouseForUpdateAsync(Guid warehouseId, CancellationToken ct)
            => await context.Warehouses
                .Include(w => w.ZonesServed)
                .FirstOrDefaultAsync(w => w.Id == warehouseId, ct);

        public async Task<List<Zone>> GetZonesByIdsAsync(IReadOnlyCollection<Guid> zoneIds, CancellationToken ct)
        {
            var ids = zoneIds.Distinct().ToList();
            return await context.Zones
                .Where(z => ids.Contains(z.Id))
                .ToListAsync(ct);
        }

        public async Task<Guid?> GetWarehouseGovernmentAsync(int governmentId, CancellationToken ct)
            => await context.Warehouses
                .AsNoTracking()
                .Where(w => w.ZonesServed.Any(z => z.City.GovernmentId == governmentId))
                .Select(w => (Guid?)w.Id)
                .FirstOrDefaultAsync(ct);

        private IQueryable<WarehouseDto> ProjectWarehouseDto(IQueryable<Warehouse> query)
            => query.Select(w => new WarehouseDto
            {
                Id = w.Id,
                Name = w.Name,
                Type = w.Type,
                Address = w.Address,
                Capacity = w.Capacity,
                CurrentUsage = w.CurrentUsage,
                OperatingHours = w.OperatingHours,
                WarehouseManagerName = w.Manager.FullName,
                ZoneIds = w.ZonesServed.Select(z => z.Id).ToList(),
                ZoneNames = w.ZonesServed.Select(z => z.Name).ToList(),
                CarrierCount = context.Carriers.Count(c => c.HomeWarehouseId == w.Id),
                ActiveTripsCount = w.Trips.Count(t => t.Status == TripStatus.Active),
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            });
    }
}
