using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.WarehouseManagerDashboardService;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
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

            var totalCarriersTask = dashboardRepository.TotalCarriersAsync(warehouseId, cancellationToken);

            var activeCarriersTask = dashboardRepository.ActiveCarriersAsync(warehouseId, cancellationToken);

            var totalShipmentsTask = dashboardRepository.TotalShipmentAsync(warehouseId, cancellationToken);

            var totalTripsTask = dashboardRepository.TotalTripsAsync(warehouseId, cancellationToken);

            var shipmentStatsTask = dashboardRepository.GetShipmentCountInStatusAsync(warehouseId, cancellationToken);

            var tripStatsTask = dashboardRepository.GetTripsCountInStatusAsync(warehouseId, cancellationToken);


            await Task.WhenAll(
                recentTripsTask,
                recentShipmentsTask,
                totalCarriersTask,
                activeCarriersTask,
                totalShipmentsTask,
                totalTripsTask,
                shipmentStatsTask,
                tripStatsTask);

            var shipmentStats = shipmentStatsTask.Result;
            var tripStats =  tripStatsTask.Result;

            var Kpi = new WarehouseManagerKpiDto
            {
                TotalShipments = totalShipmentsTask.Result,
                InTransitShipments = GetShipmentCount(shipmentStats, ShipmentStatuses.InTransit),
                DeliveredShipments = GetShipmentCount(shipmentStats, ShipmentStatuses.Delivered),
                TotalCarriers = totalCarriersTask.Result,
                ActiveCarriers = activeCarriersTask.Result,
                TotalTrips = totalTripsTask.Result,
                ActiveTrips = GetTripCount(tripStats, TripStatus.Active),
                CompletedTrips = GetTripCount(tripStats, TripStatus.Completed),

            };

            var dashboard = new WarehouseManagerDashboardDto
            {
                Manager = manager,
                Kpis = Kpi,
                RecentShipments =[ ..recentShipmentsTask.Result],
                RecentTrips = [.. recentTripsTask.Result]
            };
            return dashboard;
        }
        private static int GetShipmentCount(Dictionary<ShipmentStatuses, int> stats, ShipmentStatuses status)
        {
            return stats.GetValueOrDefault(status);
        }

        private static int GetTripCount(Dictionary<TripStatus, int> stats, TripStatus status)
        {
            return stats.GetValueOrDefault(status);
        }

    }
}

