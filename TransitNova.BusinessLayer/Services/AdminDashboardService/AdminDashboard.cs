

using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.Interfaces.Repositories.Admin;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Services.AdminDashboardService
{
    internal class AdminDashboard(IAdminActivityQueryRepository adminActivity ,  
             IAdminOperationalHealth adminOperations, IAdminStatisticsQueryRepository adminStatistics) : IAdminDashboard
    {
        public async Task<AdminDashboardDto> BuildAsync(CancellationToken cancellationToken)
        {

            var recentActivityTask = adminActivity.GetRecentActivitiesAsync(cancellationToken);
            var recentShipmentsTask = adminActivity.GetRecentShipmentsAsync(cancellationToken);
            var shipmentStatsTask = adminActivity.GetShipmentCountInStatus(cancellationToken);
            var topCarriersTask = adminActivity.GetTopCarriersAsync(cancellationToken);
            var topOperationManagersTask = adminActivity.GetTopOperationManagersAsync(cancellationToken);

            var totalCarriersTask = adminStatistics.GetTotalCarriersAsync(cancellationToken);
            var totalShipmentsTask = adminStatistics.GetTotalShipmentsCountAsync(cancellationToken);
            var totalUsersTask = adminStatistics.GetTotalUsersCountAsync(cancellationToken);
            var totalOperationManagersTask = adminStatistics.GetTotalOperationManagersCountAsync(cancellationToken);
            var activeTripsTask = adminStatistics.GetTotalActiveTripsAsync(cancellationToken);

            var activeOperationManagersTask = adminOperations.ActiveOperationManagersAsync(cancellationToken);
            var averageCarrierRatingTask = adminOperations.AverageCarrierRatingAsync(cancellationToken);
            var busyCarriersTask = adminOperations.BusyCarriersAsync(cancellationToken);
            var cancelledShipmentsRateTask = adminOperations.CancelledShipmentRateAsync(cancellationToken);
            var activeCarriersTask = adminOperations.GetActiveCarriersCountAsync(cancellationToken);
            var deliverySuccessRateTask = adminOperations.DeliverySuccessRateAsync(cancellationToken);
            var activeShipmentsTask = adminOperations.ActiveShipmentAsync(cancellationToken);
            var availableCarriersTask = adminOperations.AvailableCarriersAsync(cancellationToken);


            await Task.WhenAll(
                recentActivityTask,
                recentShipmentsTask,
                shipmentStatsTask,
                topCarriersTask,
                topOperationManagersTask,
                totalCarriersTask,
                totalShipmentsTask,
                totalUsersTask,
                totalOperationManagersTask,
                activeTripsTask,
                activeOperationManagersTask,
                averageCarrierRatingTask,
                busyCarriersTask,
                cancelledShipmentsRateTask,
                activeCarriersTask,
                activeShipmentsTask,
                availableCarriersTask,
                deliverySuccessRateTask);

            var recentActivity = recentActivityTask.Result;
            var recentShipments =  recentShipmentsTask.Result;
            var shipmentStats =  shipmentStatsTask.Result;
            var topCarriers =  topCarriersTask.Result;
            var topOperationManagers =topOperationManagersTask.Result;

            var totalCarriers = totalCarriersTask.Result;
            var totalShipments = totalShipmentsTask.Result;
            var totalUsers = totalUsersTask.Result;
            var totalOperationManagers = totalOperationManagersTask.Result;
            var activeTrips = activeTripsTask.Result;
            var activeShipments = activeShipmentsTask.Result;



            var activeOperationManagers = activeOperationManagersTask.Result;
            var averageCarrierRating = averageCarrierRatingTask.Result;
            var busyCarriers = busyCarriersTask.Result;
            var availableCarriers = availableCarriersTask.Result;
            var cancelledShipmentsRate = cancelledShipmentsRateTask.Result;
            var activeCarriers = activeCarriersTask.Result;
            var deliverySuccessRate = deliverySuccessRateTask.Result;



            var shipmentStatusDetails = BuildShipmentStatusStatistics(shipmentStats);

            var kpis = new AdminKpiDto()
            {
                TotalShipments = totalShipments,
                TotalCarriers = totalCarriers,
                TotalOperationManagers = totalOperationManagers,
                TotalUsers = totalUsers,
                ActiveTrips = activeTrips,
                ActiveShipments = activeShipments,
                PendingShipments = CalculateShipmentsInStatus(shipmentStats, ShipmentStatuses.Pending),
                DeliveredShipments = CalculateShipmentsInStatus(shipmentStats, ShipmentStatuses.Delivered)
            };

            var operationHealth = new AdminOperationalHealthDto()
            {
                ActiveOperationManagers = activeOperationManagers,
                AvailableCarriers = availableCarriers,
                AverageCarrierRating = averageCarrierRating,
                BusyCarriers = busyCarriers,
                CancelledShipmentRate = cancelledShipmentsRate,
                DeliverySuccessRate = deliverySuccessRate,
            };


            var dashboard = new AdminDashboardDto()
            {

                Kpis = kpis,
                ShipmentStatistics = shipmentStatusDetails,
                RecentShipments = recentShipments,
                TopCarriers = topCarriers,
                TopOperationManagers = topOperationManagers,
                RecentActivities = recentActivity,
                OperationalHealth = operationHealth,
            };

            return dashboard;
        }

        static int CalculateShipmentsInStatus(Dictionary<ShipmentStatuses, int> stats, ShipmentStatuses status)
        {
            var count = stats.GetValueOrDefault(status);
            return count;
        }

        List<ShipmentStatusStatDto> BuildShipmentStatusStatistics(Dictionary<ShipmentStatuses, int> stats)
        {
            return stats.Select(s => new ShipmentStatusStatDto
            {
                Status = s.Key,
                Count = s.Value
            } ).ToList();
        }

     }
}