using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string GovernmentName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string? Code { get; set; }
    public DateTime ContractStartDate { get; set; }
    public DateTime ContractEndDate { get; set; }
    public decimal Rating { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public int Experience { get; set; }
    public decimal DefaultCostPerKg { get; set; }
    public CarrierStatus Status { get; set; }
    public UiCarrierVehicleDto? Vehicle { get; set; }

    public static UiCarrierProfileDto ToUiDto(CarrierProfileDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            UserType = source.UserType,
            CityName = source.CityName,
            GovernmentName = source.GovernmentName,
            CountryName = source.CountryName,
            Code = source.Code,
            ContractStartDate = source.ContractStartDate,
            ContractEndDate = source.ContractEndDate,
            Rating = source.Rating,
            LicenseNumber = source.LicenseNumber,
            Experience = source.Experience,
            DefaultCostPerKg = source.DefaultCostPerKg,
            Status = source.Status,
            Vehicle = source.Vehicle is null ? null : UiCarrierVehicleDto.ToUiDto(source.Vehicle)
        };
}
