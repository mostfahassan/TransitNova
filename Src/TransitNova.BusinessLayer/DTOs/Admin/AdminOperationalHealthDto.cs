
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{


    public sealed class AdminOperationalHealthDto
    {
        public int AvailableCarriers { get; set; }

        public int BusyCarriers { get; set; }

        public int ActiveOperationManagers { get; set; }

        public decimal AverageCarrierRating { get; set; }

        public decimal DeliverySuccessRate { get; set; }

        public decimal CancelledShipmentRate { get; set; }
    }

}
