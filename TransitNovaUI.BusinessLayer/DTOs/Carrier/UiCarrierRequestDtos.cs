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

public sealed record UiAssignCarrierDto(Guid CarrierId)
{
    public static AssignCarrierDto ToDto(UiAssignCarrierDto source) => new(source.CarrierId);

}

public sealed record UiRatingCarrierDto(Guid CarrierId, string? Comment, int Rating)
{
    public static RatingCarrierDto ToDto(UiRatingCarrierDto source) =>
        new(source.CarrierId, source.Comment, source.Rating);

}

public sealed record UiChangeCarrierStatusDto(Guid Id, CarrierStatus Status)
{
    public static ChangeCarrierStatus ToDto(UiChangeCarrierStatusDto source) =>
        new(source.Id, source.Status);

}

public sealed class UiUpdateCarrierDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public int? CityId { get; set; }
    public string? Address { get; set; } = string.Empty;
 

    public static UpdateCarrierDto ToDto(UiUpdateCarrierDto source) =>
        new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            CityId = source.CityId,
            Address = source.Address,
           
        };

}
