
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class AdditionalInfoDto
    {
        public Guid Id { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int MaxDailyShipments { get; set; }
        public decimal DefaultCostPerKg { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime ContractStartDate { get; set; }
        public int ContractYears { get; set; }
        public Guid WarehouseId { get; set; }
    }
}
