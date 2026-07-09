using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.WarehouseManagerRepo
{
    internal sealed class WarehouseManagerRulesRepository(AppDbContext context) : IWarehouseManagerRuleseRepository
    {
        public async Task<bool> ExistsAsync(Guid managerId, CancellationToken ct = default)
        => await context.WarehouseManagersProfiles.AnyAsync(manager =>manager.Id == managerId, ct);
        public async Task<bool> IsWarehouseManager(Guid managerId, Guid warehouseId, CancellationToken ct = default)
         => await context.WarehouseManagersProfiles.AnyAsync(manager =>
            manager.AppUserId == managerId &&
            manager.Warehouse != null &&
            manager.Warehouse.Id == warehouseId,
            ct);
    }
}
