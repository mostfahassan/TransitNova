
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{

    public sealed class AdminKpiDto
    {
        public int TotalShipments { get; set; }

        public int ActiveShipments { get; set; }

        public int DeliveredShipments { get; set; }

        public int PendingShipments { get; set; }

        public int TotalUsers { get; set; }

        public int TotalCarriers { get; set; }

        public int TotalOperationManagers { get; set; }

        public int ActiveTrips { get; set; }
    }

}
