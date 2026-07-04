using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.Features.Carriers
{
    public static class CarrierDashboardBuilder
    {
        public static CarrierDashboardDto Build(Dictionary<ShipmentStatuses, int> stats,
            List<CarrierTripDto> trips, decimal totalRevenue, Carrier carrier, IEnumerable<RetrieveShipmentDto> recentShipments)
        {
            var assigned = stats.GetValueOrDefault(ShipmentStatuses.AssignedToPickUpCarrier)
                + stats.GetValueOrDefault(ShipmentStatuses.AssignedToDeliveryCarrier)
                + stats.GetValueOrDefault(ShipmentStatuses.OutForPickup)
                + stats.GetValueOrDefault(ShipmentStatuses.OutForDelivery)
                + stats.GetValueOrDefault(ShipmentStatuses.InTransit);

            var activeTrips = trips
                .Where(t => t.Status is TripStatus.Active or TripStatus.Planned)
                .OrderBy(t => t.PlannedDate)
                .Take(5)
                .ToList();
            return new CarrierDashboardDto
            {
                Profile = CarrierProfileBuilder.FromCarrier(carrier),
                AssignedShipmentsCount = assigned,
                DeliveredShipmentsCount = stats.GetValueOrDefault(ShipmentStatuses.Delivered),
                PendingShipmentsCount = stats.GetValueOrDefault(ShipmentStatuses.Pending),
                ActiveTripsCount = activeTrips.Count,
                RevenueSummary = totalRevenue,
                RecentShipments = [..recentShipments],
                ActiveTrips = activeTrips,
                ShipmentStatistics = [.. stats.Select(s => new CarrierStatusStatDto { Status = s.Key, Count = s.Value })],
                RecentActivity = [..recentShipments
                  .Take(6)
                  .Select(sh => new CarrierActivityDto
                  {
                      Title = sh.TrackingNumber,
                      Description = $"{sh.CurrentStatus} shipment for {sh.Receiver.FullName}",
                      Status = sh.CurrentStatus,
                      OccurredAt = sh.CreatedAt
                  })]
                  
            };

        }
    }
}
