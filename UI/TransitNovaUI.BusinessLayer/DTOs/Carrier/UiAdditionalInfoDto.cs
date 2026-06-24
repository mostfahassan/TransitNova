using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiAdditionalInfoDto
{
    public Guid Id { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public int MaxDailyShipments { get; set; }
    public decimal DefaultCostPerKg { get; set; }
    public int YearsOfExperience { get; set; }
    public DateTime ContractStartDate { get; set; }
    public int ContractYears { get; set; }
    public Guid WarehouseId { get; set; }

    public static AdditionalInfoDto ToDto(UiAdditionalInfoDto source) =>
        new()
        {
            Id = source.Id,
            LicenseNumber = source.LicenseNumber,
            MaxDailyShipments = source.MaxDailyShipments,
            DefaultCostPerKg = source.DefaultCostPerKg,
            YearsOfExperience = source.YearsOfExperience,
            ContractStartDate = source.ContractStartDate,
            ContractYears = source.ContractYears,
            WarehouseId = source.WarehouseId
        };

}
