using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Repository.Generic;
namespace TransitNova.InfraStructure.Repository.WarehouseRepo
{
    internal class WarehouseRulesRepository(AppDbContext context, IMapper mapper) : GenericRepository<Warehouse, Guid>(context, mapper.ConfigurationProvider), IWarehouseRulesRepository
    {
        public async Task<bool> ExistsByNameAsync(string name, Guid? excludedWarehouseId, CancellationToken ct)
        {
            var normalizedName = name.Trim().ToLower();

            return await context.Warehouses
                .AsNoTracking()
                .AnyAsync(w =>
                    w.Name.ToLower() == normalizedName &&
                    (!excludedWarehouseId.HasValue || w.Id != excludedWarehouseId.Value),
                    ct);
        }
    }

}
