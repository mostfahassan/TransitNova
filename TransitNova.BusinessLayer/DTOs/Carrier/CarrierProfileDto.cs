
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class CarrierProfileDto : CommonRetrieveData
    {
        public string? Code { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public decimal Rating { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int Experience { get; set; }
        public decimal DefaultCostPerKg { get; set; }
        public CarrierStatus Status { get; set; }
        public CarrierVehicleDto? Vehicle { get; set; }
    }
}
