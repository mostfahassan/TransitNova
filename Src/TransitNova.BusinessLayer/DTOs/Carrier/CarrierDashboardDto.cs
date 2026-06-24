using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class CarrierDashboardDto
    {
        public CarrierProfileDto Profile { get; set; } = new();
        public int AssignedShipmentsCount { get; set; }
        public int DeliveredShipmentsCount { get; set; }
        public int PendingShipmentsCount { get; set; }
        public int ActiveTripsCount { get; set; }
        public decimal RevenueSummary { get; set; }
        public IReadOnlyCollection<CarrierStatusStatDto> ShipmentStatistics { get; set; } = [];
        public IReadOnlyCollection<RetrieveShipmentDto> RecentShipments { get; set; } = [];
        public IReadOnlyCollection<CarrierTripDto> ActiveTrips { get; set; } = [];
        public IReadOnlyCollection<CarrierActivityDto> RecentActivity { get; set; } = [];
    }

}
