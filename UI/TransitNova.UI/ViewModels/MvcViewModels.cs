using System.ComponentModel.DataAnnotations;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;
using TransitNovaUI.BusinessLayer.Common.CommonData;
using TransitNovaUI.BusinessLayer.DTOs.Bundle;
using TransitNovaUI.BusinessLayer.DTOs.Carrier;
using TransitNovaUI.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.DTOs.Country;
using TransitNovaUI.BusinessLayer.DTOs.Roles;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Trips;
using TransitNovaUI.BusinessLayer.DTOs.UserProfile;
using TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNovaUI.BusinessLayer.DTOs.Vehicle;
using TransitNovaUI.BusinessLayer.DTOs.Warehouse;
using TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

namespace TransitNova.UI.ViewModels;

public sealed class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public UiLoginDto ToDto() => new(Password, Email);
}

public sealed class RegisterViewModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public UserType UserType { get; set; } = UserType.User;

    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    [Range(1, int.MaxValue)]
    public int GovernmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int CityId { get; set; }

    public IReadOnlyCollection<LocationOptionViewModel> Countries { get; set; } = [];
    public IReadOnlyCollection<LocationOptionViewModel> Governments { get; set; } = [];
    public IReadOnlyCollection<LocationOptionViewModel> Cities { get; set; } = [];

    public UiRegisterDto ToDto() => new()
    {
        UserName = UserName,
        Email = Email,
        Password = Password,
        ConfirmPassword = ConfirmPassword,
        PhoneNumber = PhoneNumber,
        FirstName = FirstName,
        LastName = LastName,
        Address = Address,
        UserType = UserType,
        CityId = CityId
    };
}

public sealed record LocationOptionViewModel(int Id, string Name);

public sealed class ChangePasswordViewModel
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;

    [Required, Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = string.Empty;

    public UiChangePasswordDto ToDto() => new(CurrentPassword, NewPassword, ConfirmNewPassword);
}

public sealed class PackageSpecificationViewModel
{
    [Range(0.01, double.MaxValue)]
    public decimal Weight { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Width { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Height { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Length { get; set; }

    public UiPackageSpecificationRequestDto ToDto() => new()
    {
        Weight = Weight,
        Width = Width,
        Height = Height,
        Length = Length
    };
}

public sealed class ReceiverViewModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    public Guid SenderId { get; set; }

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CityId { get; set; }

    public UiCreateReceiverDto ToDto(Guid senderId)
    {
        SenderId = senderId;
        return new UiCreateReceiverDto
        {
            FirstName = FirstName,
            SenderId = senderId,
            LastName = LastName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Address = Address,
            CityId = CityId
        };
    }
}

public sealed class CreateShipmentViewModel
{
    public ReceiverViewModel Receiver { get; set; } = new();
    public PackageSpecificationViewModel PackageSpecification { get; set; } = new();
    public Currency Currency { get; set; }
    public DateTime? PickUpDate { get; set; }
    public TransportationMode TransportationMode { get; set; }
    public enShipmentType ShipmentDeliveryType { get; set; }

    [Required]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Required]
    public string PickupAddress { get; set; } = string.Empty;

    public Guid? PackageBundleId { get; set; }
    public Guid PaymentId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public UiCreateShipmentDto ToDto(Guid senderId) => new(
        Receiver.ToDto(senderId),
        PackageSpecification.ToDto(),
        Currency,
        PickUpDate,
        TransportationMode,
        ShipmentDeliveryType,
        DeliveryAddress,
        PickupAddress,
        PackageBundleId,
        PaymentId,
        PaymentMethod);
}

public sealed class UpdateShipmentViewModel
{
    public Guid? ReceiverId { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? PickupAddress { get; set; }
    public PackageSpecificationViewModel? PackageSpecification { get; set; }
    public enShipmentType? ShipmentType { get; set; }
    public TransportationMode? TransportationMode { get; set; }

    public UiUpdateShipmentDto ToDto() => new(
        ReceiverId,
        DeliveryAddress,
        PickupAddress,
        PackageSpecification?.ToDto(),
        ShipmentType,
        TransportationMode);
}

public sealed class ShipmentFilterViewModel
{
    public ShipmentStatuses[]? Status { get; set; }
    public TransportationMode? Mode { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Guid? SenderId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public UiShipmentFilterDto ToDto() => new()
    {
        Status = Status,
        Mode = Mode,
        From = From,
        To = To,
        SenderId = SenderId,
        SearchTerm = SearchTerm,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}

public sealed class CityFilterViewModel
{
    public int? GovernmentId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool SortDescending { get; set; }

    public UiCityFilterDto ToDto() => new()
    {
        GovernmentId = GovernmentId,
        SearchTerm = SearchTerm,
        PageNumber = PageNumber,
        PageSize = PageSize,
        SortDescending = SortDescending
    };
}

public sealed class UserFilterViewModel
{
    public string? SearchTerm { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public UiUserFiltrationDto ToDto() => new()
    {
        SearchTerm = SearchTerm,
        Email = Email,
        UserName = UserName,
        PhoneNumber = PhoneNumber,
        IsActive = IsActive,
        CreatedFrom = CreatedFrom,
        CreatedTo = CreatedTo,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}

public sealed class WarehouseManagerFilterViewModel
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public Guid? WarehouseId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public UiWarehouseManagerFilterDto ToDto() => new()
    {
        FullName = FullName,
        Email = Email,
        WarehouseId = WarehouseId,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}
public sealed class CarrierFilterViewModel
{
    public CarrierStatus? Status { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public int? MinYearsOfExperience { get; set; }
    public int? MaxYearsOfExperience { get; set; }
    public string? City { get; set; }
    public int? CityId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public CarrierSortBy? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public decimal? VehicleCapacityWeight { get; set; }
    public VehicleType? VehicleType { get; set; }
    public List<string>? ServedZones { get; set; }

    public UiFilterCarrierDto ToDto() => new()
    {
        Status = Status,
        MinRating = MinRating,
        MaxRating = MaxRating,
        MinYearsOfExperience = MinYearsOfExperience,
        MaxYearsOfExperience = MaxYearsOfExperience,
        City = City,
        CityId = CityId,
        SearchTerm = SearchTerm,
        AvailableFrom = AvailableFrom,
        PageNumber = PageNumber,
        PageSize = PageSize,
        SortBy = SortBy,
        SortDescending = SortDescending,
        VehicleCapacityWeight = VehicleCapacityWeight,
        VehicleType = VehicleType,
        ServedZones = ServedZones
    };
}

public sealed class TripFilterViewModel
{
    public Guid? Id { get; set; }
    public TripType? TripType { get; set; }
    public TripStatus[]? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? CreatedBy { get; set; }
    public Guid? CarrierId { get; set; }
    public Guid? WarehouseId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public UiFilterTripsDto ToDto() => new()
    {
        Id = Id,
        TripType = TripType,
        Status = Status,
        CreatedAt = CreatedAt,
        From = From,
        To = To,
        CreatedBy = CreatedBy,
        CarrierId = CarrierId,
        WarehouseId = WarehouseId,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}



public sealed class CarrierShipmentFilterViewModel
{
    public ShipmentStatuses? Status { get; set; }
    public TransportationMode? Mode { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public UiCarrierShipmentFilterDto ToDto() => new()
    {
        Status = Status,
        Mode = Mode,
        SearchTerm = SearchTerm,
        SortBy = SortBy,
        SortDescending = SortDescending,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}
public sealed class RateCalculatorViewModel
{
    public PackageSpecificationViewModel PackageSpecification { get; set; } = new();
    public TransportationMode TransportationMode { get; set; }
    public enShipmentType ShipmentDeliveryType { get; set; }

    public UiRateCalculatorDto ToDto() => new()
    {
        PackageSpecification = PackageSpecification.ToDto(),
        TransportationMode = TransportationMode,
        ShipmentDeliveryType = ShipmentDeliveryType
    };
}

public sealed class ReasonViewModel
{
    [Required]
    public string Reason { get; set; } = string.Empty;

    public UiIssueShipmentReason ToIssueDto() => new(Reason);

    public UiRejectShipmentReason ToRejectDto() => new(Reason);
}

public sealed class AssignCarrierViewModel
{
    [Required]
    public Guid CarrierId { get; set; }

    public UiAssignCarrierDto ToDto() => new(CarrierId);
}

public sealed class BundleFormViewModel
{
    public Guid? BundleId { get; set; }

    [Required]
    public string BundleName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal TotalWeight { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal BundlePrice { get; set; }

    [Required]
    public string BundleDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal TotalDistance { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalShipments { get; set; }

    public UiCreateBundleDto ToCreateDto() => new()
    {
        BundleName = BundleName,
        TotalWeight = TotalWeight,
        BundlePrice = BundlePrice,
        BundleDescription = BundleDescription,
        TotalDistance = TotalDistance,
        TotalShipments = TotalShipments
    };

    public UiUpdateBundleDto ToUpdateDto(Guid bundleId) => new()
    {
        BundleId = bundleId,
        BundleName = BundleName,
        TotalWeight = TotalWeight,
        BundlePrice = BundlePrice,
        BundleDescription = BundleDescription,
        TotalDistance = TotalDistance,
        TotalShipments = TotalShipments
    };
}

public sealed class CityFormViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int GovernmentId { get; set; }

    public UiCreateCityDto ToCreateDto() => new() { Name = Name, GovernmentId = GovernmentId };

    public UiUpdateCityDto ToUpdateDto() => new() { Name = Name, GovernmentId = GovernmentId };
}

public sealed class GovernmentFormViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    public UiCreateGovernmentDto ToCreateDto() => new() { Name = Name, CountryId = CountryId };

    public UiUpdateGovernmentDto ToUpdateDto() => new() { Name = Name, CountryId = CountryId };
}

public sealed class WarehouseFormViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public WarehouseType Type { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Capacity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CurrentUsage { get; set; }

    public int? OperatingHours { get; set; }
    public IReadOnlyCollection<Guid> ZoneIds { get; set; } = [];

    public UiCreateWarehouseDto ToCreateDto() => new()
    {
        Name = Name,
        Type = Type,
        Address = Address,
        Capacity = Capacity,
        CurrentUsage = CurrentUsage,
        OperatingHours = OperatingHours ?? 0,
        ZoneIds = ZoneIds
    };

    public UiUpdateWarehouseDto ToUpdateDto() => new()
    {
        Name = Name,
        Type = Type,
        Address = Address,
        Capacity = Capacity,
        CurrentUsage = CurrentUsage,
        OperatingHours = OperatingHours,
        ZoneIds = ZoneIds
    };
}

public sealed class RoleFormViewModel
{
    [Required]
    public string RoleName { get; set; } = string.Empty;

    public UiRoleNameDto ToDto() => new(RoleName);
}

public sealed class RoleMembersFormViewModel
{
    public IReadOnlyCollection<RoleMemberFormViewModel> Users { get; set; } = [];

    public UiUpdateRoleMembersDto ToDto() => new()
    {
        Users = Users.Select(user => new UiRoleMemberUpdateDto
        {
            UserId = user.UserId,
            IsInRole = user.IsInRole
        }).ToList()
    };
}

public sealed class RoleMemberFormViewModel
{
    public Guid UserId { get; set; }
    public bool IsInRole { get; set; }
}

public sealed class VehicleFormViewModel
{
    public VehicleType VehicleType { get; set; }

    [Required]
    public string PlateNumber { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal CapacityWeight { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal CapacityVolume { get; set; }

    public bool IsRefrigerated { get; set; }

    [Required]
    public Guid CarrierId { get; set; }

    public UiCreateVehicleDto ToDto() => new()
    {
        VehicleType = VehicleType,
        PlateNumber = PlateNumber,
        CapacityWeight = CapacityWeight,
        CapacityVolume = CapacityVolume,
        IsRefrigerated = IsRefrigerated,
        CarrierId = CarrierId
    };
}

public sealed class WarehouseManagerProfileFormViewModel
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }
    public Guid? WarehouseId { get; set; }

    public UiUpdateWarehouseManagerProfileDto ToDto() => new()
    {
        Id = Id,
        FirstName = FirstName,
        LastName = LastName,
        PhoneNumber = PhoneNumber,
        Email = Email,
        CityId = CityId,
        Address = Address,
        WarehouseId = WarehouseId
    };
}



public sealed class CarrierProfileFormViewModel
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int? CityId { get; set; }
    public string? Address { get; set; }

    public UiUpdateCarrierDto ToDto(Guid carrierId) => new()
    {
        Id = carrierId,
        FirstName = FirstName,
        LastName = LastName,
        PhoneNumber = PhoneNumber,
        Email = Email,
        CityId = CityId,
        Address = Address
    };
}

public sealed class CarrierAdditionalInfoFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string LicenseNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int MaxDailyShipments { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal DefaultCostPerKg { get; set; }

    [Range(0, int.MaxValue)]
    public int YearsOfExperience { get; set; }

    public DateTime ContractStartDate { get; set; } = DateTime.UtcNow;

    [Range(1, int.MaxValue)]
    public int ContractYears { get; set; }

    public Guid WarehouseId { get; set; }

    public UiAdditionalInfoDto ToDto(Guid carrierId) => new()
    {
        Id = carrierId,
        LicenseNumber = LicenseNumber,
        MaxDailyShipments = MaxDailyShipments,
        DefaultCostPerKg = DefaultCostPerKg,
        YearsOfExperience = YearsOfExperience,
        ContractStartDate = ContractStartDate,
        ContractYears = ContractYears,
        WarehouseId = WarehouseId
    };
}

public sealed class CarrierStatusFormViewModel
{
    public CarrierStatus Status { get; set; }

    public UiChangeCarrierStatusDto ToDto() => new(Status);
}

