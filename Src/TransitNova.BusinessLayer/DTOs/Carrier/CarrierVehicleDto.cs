using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{

    public class CarrierVehicleDto
    {
        public Guid Id { get; set; }
        public VehicleType VehicleType { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public decimal CapacityWeight { get; set; }
        public decimal CapacityVolume { get; set; }
        public bool IsRefrigerated { get; set; }
        public bool IsActive { get; set; }
    }

}
