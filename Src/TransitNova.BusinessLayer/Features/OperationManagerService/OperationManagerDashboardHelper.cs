using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Features.OperationManagerService
{
    internal static  class OperationManagerDashboardHelper
    {
        public static OperationManagerDashboardDto Build(
            IEnumerable <RetrieveShipmentDto> shipmentData,
            Dictionary<ShipmentStatuses, int> shipmentStats)
        {
            var pending = CalculateShipmentsInStatus(shipmentStats, ShipmentStatuses.Pending);

            var delivered = CalculateShipmentsInStatus(shipmentStats, ShipmentStatuses.Delivered);

            var inTransit = CalculateShipmentsInStatus(shipmentStats, ShipmentStatuses.InTransit);

            var inWarehouse = CalculateShipmentsInStatus(shipmentStats, ShipmentStatuses.InWarehouse);

            var recentShipments = shipmentData.ToList();

            return new OperationManagerDashboardDto()
            {
                TotalShipments = shipmentData.Count(),

                PendingShipments = pending,

                DeliveredShipments = delivered,
                ActiveShipments = CalculateAssignedShipments(shipmentStats),

                RecentShipments = recentShipments,

                ShipmentStatistics =
                [
                    new() { Status = ShipmentStatuses.Pending, Count = pending },

                    new() { Status = ShipmentStatuses.InTransit, Count = inTransit },

                    new() { Status = ShipmentStatuses.InWarehouse, Count = inWarehouse },

                    new() { Status = ShipmentStatuses.Delivered, Count = delivered }
                ],

                RecentActivity = 
                [.. recentShipments
                    .Take(5)
                    .Select(s => new OperationManagerActivityDto
                    {
                        Title = s.TrackingNumber,

                        Description = $"{s.CurrentStatus} shipment for {s.Receiver.FullName}",

                        Status = s.CurrentStatus,

                        OccurredAt = s.CreatedAt
                    })],
           
            };
        }
   
         static int CalculateAssignedShipments(Dictionary<ShipmentStatuses, int> stats)
        {
            var activeShipments =
                stats.GetValueOrDefault(ShipmentStatuses.AssignedToPickUpCarrier)
                + stats.GetValueOrDefault(ShipmentStatuses.AssignedToDeliveryCarrier)
                + stats.GetValueOrDefault(ShipmentStatuses.OutForPickup)
                + stats.GetValueOrDefault(ShipmentStatuses.OutForDelivery)
                + stats.GetValueOrDefault(ShipmentStatuses.InWarehouse)
                + stats.GetValueOrDefault(ShipmentStatuses.InTransit);

            return activeShipments;
        }
        static int CalculateShipmentsInStatus(Dictionary<ShipmentStatuses, int> stats, ShipmentStatuses status)
        {
            var count = stats.GetValueOrDefault(status);
            return count;
        }
    }
}
