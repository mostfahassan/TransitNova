using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.WarehouseManagerRepo
{
    internal sealed class WarehouseManagerCommandRepository(AppDbContext context) : IWarehouseManagerCommandRepository
    {
        public async Task<WarehouseManagerProfile?> GetByIdForUpdateAsync(Guid managerId, CancellationToken ct = default)
            => await context.WarehouseManagersProfiles
                .Include(manager => manager.Warehouse)
                .FirstOrDefaultAsync(manager => manager.Id == managerId || manager.AppUserId == managerId, ct);

    }
}
