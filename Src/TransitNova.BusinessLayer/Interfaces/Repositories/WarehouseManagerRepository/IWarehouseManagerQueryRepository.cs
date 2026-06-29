using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository
{
    public interface IWarehouseManagerQueryRepository
    {
        Task<WarehouseManagerDetailsDto?> GetByIdAsync(Guid managerId, CancellationToken ct = default);
        Task<PagedResult<WarehouseManagerListDto>> GetAllWarehousesAsync(WarehouseManagerFilterDto filter, CancellationToken ct = default);
        Task<Guid?> GetWarehouseIdAsync(Guid managerId, CancellationToken ct = default);
    }
}

