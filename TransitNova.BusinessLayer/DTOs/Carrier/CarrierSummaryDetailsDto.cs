using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class CarrierSummaryDetailsDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Code { get; set; }
        public CarrierStatus Status { get; set; }
        public List<string> ServedCities { get; set; } = [];
        public int AssignedShipmentsCount { get; set; }
        public int ActiveTripsCount { get; set; }
        public decimal Rating { get; set; }
    }
}
