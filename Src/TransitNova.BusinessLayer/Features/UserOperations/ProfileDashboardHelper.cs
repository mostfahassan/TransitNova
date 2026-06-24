using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.UserOperations
{
    public static class ProfileDashboardHelper
    {
        public static ProfileDashboardDto Build(
            IEnumerable<RetrieveShipmentSummaryDto> shipments,
            IReadOnlyDictionary<ShipmentStatuses, int> statusCounts)
        {
            var shipmentList = shipments.ToList();

            return new ProfileDashboardDto
            {
                TotalShipments = shipmentList.Count,

                PendingShipments = statusCounts.GetValueOrDefault(ShipmentStatuses.Pending),

                DeliveredShipments = statusCounts.GetValueOrDefault(ShipmentStatuses.Delivered),

                ActiveShipments =
                    statusCounts.GetValueOrDefault(ShipmentStatuses.InTransit)
                  + statusCounts.GetValueOrDefault(ShipmentStatuses.OutForPickup)
                  + statusCounts.GetValueOrDefault(ShipmentStatuses.OutForDelivery),

                IssueShipments =
                    statusCounts.GetValueOrDefault(ShipmentStatuses.Issue)
                  + statusCounts.GetValueOrDefault(ShipmentStatuses.Cancelled) 
                  + statusCounts.GetValueOrDefault(ShipmentStatuses.Rejected),

                ShipmentSummary = shipmentList
            };
        }
        public static ProfileDashboardDto Empty()
        {
            return new ProfileDashboardDto();
        }
    }

}
