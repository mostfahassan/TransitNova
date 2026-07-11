using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository
{
    public interface IWarehouseManagerDashboardRepository
    {
        Task<WarehouseManagerSummary?> GetWarehouseManagerSummaryAsync(Guid managerId , CancellationToken cancellationToken );
        Task<IEnumerable<RecentTripSummary>> GetRecentTripsSummaryAsync(Guid warehouseId ,CancellationToken cancellationToken);
        Task<IEnumerable<RecentShipmentSummary>> GetRecentShipmentSummaryAsync(Guid warehouseId ,CancellationToken cancellationToken);
        Task<WarehouseManagerKpiDto> GetWarehouseStatsAsync(Guid warehouseId, CancellationToken cancellationToken);

    }
}

