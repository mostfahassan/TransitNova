using TransitNova.Domain.Enums.Carrier;

namespace TransitNova.BusinessLayer.DTOs.Vehicle
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public VehicleType VehicleType { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public decimal CapacityWeight { get; set; }
        public decimal CapacityVolume { get; set; }
        public bool IsRefrigerated { get; set; }
        public bool IsActive { get; set; }
        public Guid CarrierId { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string CarrierCode { get; set; } = string.Empty;
        public decimal CarrierRating { get; set; }
        public CarrierStatus CarrierStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
