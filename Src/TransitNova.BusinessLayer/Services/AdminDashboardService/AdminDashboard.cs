

using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Services.AdminDashboardService
{
    internal class AdminDashboard(IAdminDashboardBuilder adminOperations) 
        : IAdminDashboard

    {
        public async Task<AdminDashboardDto> BuildAsync(CancellationToken cancellationToken)
        {

            //=== Activity
            var recentActivityTask = adminOperations.GetRecentActivitiesAsync(cancellationToken);
            var recentShipmentsTask = adminOperations.GetRecentShipmentsAsync(cancellationToken);
            var shipmentStatsTask = adminOperations.GetShipmentCountInStatusAsync(cancellationToken);
            var topCarriersTask = adminOperations.GetTopCarriersAsync(cancellationToken);
            var topOperationManagersTask = adminOperations.GetTopOperationManagersAsync(cancellationToken);
            var RevenueSummaryTask = adminOperations.GetRevenueSummaryAsync(cancellationToken);

            // Statistics 
            var UsersStatsTask = adminOperations.UsersStatsAsync(cancellationToken);
            var TripsTask = adminOperations.TripsStatsAsync(cancellationToken);
            var operationManagerStatsTask = adminOperations.OperationManagerStatsAsync(cancellationToken);
            var shipmentStatTask  = adminOperations.ShipmentsStatsAsync(cancellationToken);
            var carrierStatsTask = adminOperations.GetCarriersActivityStatsAsync(cancellationToken);
           

            await Task.WhenAll(
                // Activity
                recentActivityTask,
                recentShipmentsTask,
                shipmentStatsTask,
                topCarriersTask,
                topOperationManagersTask,
                RevenueSummaryTask,

                // Statistics
                UsersStatsTask,
                TripsTask,
                operationManagerStatsTask,
                shipmentStatTask,
                carrierStatsTask
             );


            //
            var recentActivity = recentActivityTask.Result;
            var recentShipments =  recentShipmentsTask.Result;
            var shipmentStatusStats =  shipmentStatsTask.Result;
            var topCarriers =  topCarriersTask.Result;
            var topOperationManagers =topOperationManagersTask.Result;
            var revenueSummary = RevenueSummaryTask.Result;


            //
            var UsersStats = UsersStatsTask.Result;
            var OperationManagersStats = operationManagerStatsTask.Result;
            var TripsStats = TripsTask.Result;
            var carrierStats = carrierStatsTask.Result;
            var shipmentStat = shipmentStatTask.Result;
          



            var shipmentStatusDetails = BuildShipmentStatusStatistics(shipmentStatusStats);

            var kpis = new AdminKpiDto()
            {
                TotalShipments = shipmentStat.TotalShipments,
                TotalCarriers = carrierStats.TotalCarriers,
                TotalOperationManagers = OperationManagersStats.TotalOperationManagers,
                TotalUsers = UsersStats.TotalUsers,
                ActiveUsers = UsersStats.ActiveUsers,
                ActiveCarriers = carrierStats.ActiveCarriers,
                ActiveTrips = TripsStats.ActiveTrips,
                PlannedTrips = TripsStats.PlannedTrips,
                CompletedTrips = TripsStats.CompletedTrips,
                ActiveShipments = shipmentStat.ActiveShipments,
                PendingShipments = shipmentStat.PendingShipments,
                DeliveredShipments = shipmentStat.DeliveredShipments
            };

            var operationHealth = new AdminOperationalHealthDto()
            {
                ActiveOperationManagers = OperationManagersStats.ActiveOperationManagers,
                AvailableCarriers = carrierStats.AvailableCarriers,
                AverageCarrierRating = carrierStats.AverageCarrierRating,
                BusyCarriers = carrierStats.BusyCarriers,
                CancelledShipmentRate = shipmentStat.CancelledShipmentRate,
                DeliverySuccessRate = carrierStats.DeliverySuccessRate,
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
                RevenueSummary = revenueSummary
            };

            return dashboard;
        }


        static List<ShipmentStatusStatDto> BuildShipmentStatusStatistics(Dictionary<ShipmentStatuses, int> stats)
        {
            return [.. stats.Select(s => new ShipmentStatusStatDto
            { 
                Status = s.Key, 
                Count = s.Value 
            })];
        }

     }
}