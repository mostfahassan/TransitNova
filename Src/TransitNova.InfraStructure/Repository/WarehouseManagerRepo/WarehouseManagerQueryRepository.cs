using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.WarehouseManagerRepo
{
    internal sealed class WarehouseManagerQueryRepository(AppDbContext context)
        : IWarehouseManagerQueryRepository
    {
        public async Task<WarehouseManagerDetailsDto?> GetByIdAsync(Guid managerId, CancellationToken ct = default)
             => await context.WarehouseManagersProfiles.AsNoTracking()
                 .Where(manager => manager.Id == managerId || manager.AppUserId == managerId)
                 .Select(manager => new WarehouseManagerDetailsDto
                 {
                     Id = manager.Id,
                     FullName = manager.FullName,
                     Email = manager.Email,
                     PhoneNumber = manager.PhoneNumber,
                     WarehouseId = manager.Warehouse != null ? manager.Warehouse.Id : Guid.Empty,
                     WarehouseName = manager.Warehouse != null ? manager.Warehouse.Name:string.Empty,
                     CreatedAt = manager.CreatedAt
                 })
                .FirstOrDefaultAsync(ct);
        public async Task<PagedResult<WarehouseManagerListDto>> GetAllWarehousesAsync(WarehouseManagerFilterDto filter, CancellationToken ct = default)
        {

            var query = context.WarehouseManagersProfiles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                var fullName = filter.FullName.Trim().ToLower();
                query = query.Where(manager =>
                    EF.Functions.Like((manager.FirstName + " " + manager.LastName).ToLower(), $"%{fullName}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                var email = filter.Email.Trim().ToLower();
                query = query.Where(manager => EF.Functions.Like(manager.Email.ToLower(), $"%{email}%"));
            }

            if (filter.WarehouseId.HasValue)
            {
                query = query.Where(manager => context.Warehouses
                    .Any(warehouse => warehouse.ManagerId == manager.Id && warehouse.Id == filter.WarehouseId.Value));
            }

            var totalCount = await query.CountAsync(ct);
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var items = await query
                .OrderBy(manager => manager.FirstName)
                .ThenBy(manager => manager.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(manager => new WarehouseManagerListDto
                {
                    Id = manager.Id,
                    FullName = manager.FullName,
                    Email = manager.Email,
                    WarehouseId = manager.Warehouse != null ? manager.Warehouse.Id : Guid.Empty,
                    WarehouseName = manager.Warehouse != null ? manager.Warehouse.Name : string.Empty,
                })
                .ToListAsync(ct);

            return PagedResult<WarehouseManagerListDto>.From(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Guid?> GetWarehouseIdAsync(Guid managerId, CancellationToken ct = default)
         => await context.Warehouses
                .AsNoTracking()
                .Where(warehouse => warehouse.ManagerId == managerId)
                .Select(warehouse => (Guid?)warehouse.Id)
                .FirstOrDefaultAsync(ct);
        
  
    }
}


