using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;

namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierVehicleDto
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal CapacityWeight { get; set; }
    public decimal CapacityVolume { get; set; }
    public bool IsRefrigerated { get; set; }
    public bool IsActive { get; set; }

    public static UiCarrierVehicleDto ToUiDto(CarrierVehicleDto source) =>
        new()
        {
            Id = source.Id,
            VehicleType = source.VehicleType,
            PlateNumber = source.PlateNumber,
            CapacityWeight = source.CapacityWeight,
            CapacityVolume = source.CapacityVolume,
            IsRefrigerated = source.IsRefrigerated,
            IsActive = source.IsActive
        };
}

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
    public string? Company { get; set; }
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
            Company = source.Company,
            Experience = source.Experience,
            DefaultCostPerKg = source.DefaultCostPerKg,
            Status = source.Status,
            Vehicle = source.Vehicle is null ? null : UiCarrierVehicleDto.ToUiDto(source.Vehicle)
        };
}

public sealed class UiCarrierSummaryDetailsDto
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

    public static UiCarrierSummaryDetailsDto ToUiDto(CarrierSummaryDetailsDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            PhoneNumber = source.PhoneNumber,
            Code = source.Code,
            Status = source.Status,
            ServedCities = [.. source.ServedCities],
            AssignedShipmentsCount = source.AssignedShipmentsCount,
            ActiveTripsCount = source.ActiveTripsCount,
            Rating = source.Rating
        };

    public static UiPagedResult<UiCarrierSummaryDetailsDto> ToUiPagedDto(
        PagedResult<CarrierSummaryDetailsDto> source) =>
        UiPagedResult<UiCarrierSummaryDetailsDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}
