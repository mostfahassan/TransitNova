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
        Task<int> TotalCarriersAsync(Guid warehouseId ,CancellationToken cancellationToken);
        Task<int> ActiveCarriersAsync(Guid warehouseId ,CancellationToken cancellationToken);
        Task<int> TotalShipmentAsync(Guid warehouseId ,CancellationToken cancellationToken);
        Task<int> TotalTripsAsync(Guid warehouseId ,CancellationToken cancellationToken);
        public Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(Guid warehouseId ,CancellationToken cancellationToken);
        public Task<Dictionary<TripStatus, int>> GetTripsCountInStatusAsync(Guid warehouseId, CancellationToken cancellationToken);
        

    }
}

