using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.WarehouseManagerDashboardService;
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Services.WarehouseManagerDashboardService
{
    public sealed class WarehouseManagerDashboardBuilder(IWarehouseManagerDashboardRepository dashboardRepository)
        : IWarehouseManagerDashboard
    {
        public async Task<WarehouseManagerDashboardDto> BuildAsync(Guid managerId, CancellationToken cancellationToken)
        {
            var manager = await dashboardRepository.GetWarehouseManagerSummaryAsync(managerId, cancellationToken)
                ?? throw new NotFoundException($"Warehouse manager '{managerId}' was not found."); 

            var warehouseId = manager.WarehouseId;

            var recentTripsTask = dashboardRepository.GetRecentTripsSummaryAsync(warehouseId, cancellationToken);

            var recentShipmentsTask = dashboardRepository.GetRecentShipmentSummaryAsync(warehouseId, cancellationToken);

            var warehouseStatTask = dashboardRepository.GetWarehouseStatsAsync(warehouseId, cancellationToken);

            await Task.WhenAll(
                recentTripsTask,
                recentShipmentsTask,
                warehouseStatTask);



            var warehouseStats = warehouseStatTask.Result;

            var Kpi = warehouseStats;

            var dashboard = new WarehouseManagerDashboardDto
            {
                Manager = manager,
                Kpis = Kpi,
                RecentShipments =[ ..recentShipmentsTask.Result],
                RecentTrips = [.. recentTripsTask.Result]
            };
            return dashboard;
        }

    }
}

