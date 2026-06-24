using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.DTOs.Vehicle
{
    public class CreateVehicleDto
    {
        public VehicleType VehicleType { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public decimal CapacityWeight { get; set; }
        public decimal CapacityVolume { get; set; }
        public bool IsRefrigerated { get; set; }
        public Guid CarrierId { get; set; }
    }

}
