using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository
{
    public interface IWarehouseQueriesRepository
    {
        Task<List<WarehouseDto>> GetWarehousesAsync(CancellationToken ct);
        Task<WarehouseDto?> GetWarehouseByIdAsync(Guid warehouseId, CancellationToken ct);
        Task<Warehouse?> GetWarehouseForUpdateAsync(Guid warehouseId, CancellationToken ct); 
        Task<List<Zone>> GetZonesByIdsAsync(IReadOnlyCollection<Guid> zoneIds, CancellationToken ct);
        Task<Guid?> GetWarehouseGovernmentAsync(int governmentId, CancellationToken ct);
    }
}
