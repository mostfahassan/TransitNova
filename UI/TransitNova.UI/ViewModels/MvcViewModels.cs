using System.ComponentModel.DataAnnotations;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Bundle;
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

    public AddressViewModel Address { get; set; } = new();

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
        Address = Address.ToDto(),
        UserType = UserType,
        CityId = CityId
    };
}

public sealed class AddressViewModel
{
    [Required, StringLength(250)]
    public string MainAddress { get; set; } = string.Empty;

    [StringLength(250)]
    public string? SecondaryAddress { get; set; }

    [Required, StringLength(150)]
    public string Street { get; set; } = string.Empty;

    public UiAddressDto ToDto() => new()
    {
        MainAddress = MainAddress,
        SecondaryAddress = SecondaryAddress,
        Street = Street
    };

    public static AddressViewModel FromDto(UiAddressDto source) => new()
    {
        MainAddress = source.MainAddress,
        SecondaryAddress = source.SecondaryAddress,
        Street = source.Street
    };

    public override string ToString() => ToDto().ToString();
}
public sealed record LocationOptionViewModel(int Id, string Name);

public sealed record WarehouseOptionViewModel(Guid Id, string Name);

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

    public AddressViewModel Address { get; set; } = new();

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
            Address = Address.ToDto(),
            CityId = CityId
        };
    }
}

public sealed class CreateShipmentViewModel
{
    public ReceiverViewModel Receiver { get; set; } = new();

    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    [Range(1, int.MaxValue)]
    public int GovernmentId { get; set; }

    public IReadOnlyCollection<LocationOptionViewModel> Countries { get; set; } = [];
    public IReadOnlyCollection<LocationOptionViewModel> Governments { get; set; } = [];
    public IReadOnlyCollection<LocationOptionViewModel> Cities { get; set; } = [];

    public PackageSpecificationViewModel PackageSpecification { get; set; } = new();
    public Currency Currency { get; set; }
    public DateTime? PickUpDate { get; set; }
    public TransportationMode TransportationMode { get; set; }
    public enShipmentType ShipmentDeliveryType { get; set; }

    [Required]
    public AddressViewModel DeliveryAddress { get; set; } = new();

    [Required]
    public AddressViewModel PickupAddress { get; set; } = new();

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
        DeliveryAddress.ToDto(),
        PickupAddress.ToDto(),
        PackageBundleId,
        PaymentId,
        PaymentMethod);
}

public sealed class UpdateShipmentViewModel
{
    public Guid? ReceiverId { get; set; }
    public AddressViewModel? DeliveryAddress { get; set; }
    public AddressViewModel? PickupAddress { get; set; }
    public PackageSpecificationViewModel? PackageSpecification { get; set; }
    public enShipmentType? ShipmentType { get; set; }
    public TransportationMode? TransportationMode { get; set; }

    public UiUpdateShipmentDto ToDto() => new(
        ReceiverId,
        DeliveryAddress?.ToDto(),
        PickupAddress?.ToDto(),
        PackageSpecification?.ToDto(),
        ShipmentType,
        TransportationMode);
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

    [Required]
    public string BundleDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal BundlePrice { get; set; }

    public BundleTier Tier { get; set; }

    [Range(1, 24)]
    public int BundleDurationMonths { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxShipmentsPerMonth { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal MaxWeightPerShipment { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal MaxDistancePerShipment { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercentage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinimumShipmentValueForDiscount { get; set; }

    public UiCreateBundleDto ToCreateDto() => new()
    {
        BundleName = BundleName,
        BundleDescription = BundleDescription,
        BundlePrice = BundlePrice,
        Tier = Tier,
        BundleDurationMonths = BundleDurationMonths,
        MaxShipmentsPerMonth = MaxShipmentsPerMonth,
        MaxWeightPerShipment = MaxWeightPerShipment,
        MaxDistancePerShipment = MaxDistancePerShipment,
        DiscountPercentage = DiscountPercentage,
        MinimumShipmentValueForDiscount = MinimumShipmentValueForDiscount
    };

    public UiUpdateBundleDto ToUpdateDto(Guid bundleId) => new()
    {
        BundleId = bundleId,
        BundleName = BundleName,
        BundleDescription = BundleDescription,
        BundlePrice = BundlePrice,
        Tier = Tier,
        BundleDurationMonths = BundleDurationMonths,
        MaxShipmentsPerMonth = MaxShipmentsPerMonth,
        MaxWeightPerShipment = MaxWeightPerShipment,
        MaxDistancePerShipment = MaxDistancePerShipment,
        DiscountPercentage = DiscountPercentage,
        MinimumShipmentValueForDiscount = MinimumShipmentValueForDiscount
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
    public string? CountryName { get; set; }

    public UiCreateGovernmentDto ToCreateDto() => new() { Name = Name, CountryId = CountryId };

    public UiUpdateGovernmentDto ToUpdateDto() => new() { Name = Name, CountryId = CountryId };
}

public sealed class WarehouseFormViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public WarehouseType Type { get; set; }

    [Required]
    public AddressViewModel Address { get; set; } = new();

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
        WarehouseAddress = Address.ToDto(),
        Capacity = Capacity,
        CurrentUsage = CurrentUsage,
        OperatingHours = OperatingHours ?? 0,
        ZoneIds = ZoneIds
    };

    public UiUpdateWarehouseDto ToUpdateDto() => new()
    {
        Name = Name,
        Type = Type,
        WarehouseAddress = Address.ToDto(),
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
    public AddressViewModel? Address { get; set; }
    public Guid? WarehouseId { get; set; }

    public UiUpdateWarehouseManagerProfileDto ToDto() => new()
    {
        Id = Id,
        FirstName = FirstName,
        LastName = LastName,
        PhoneNumber = PhoneNumber,
        Email = Email,
        CityId = CityId,
        Address = Address?.ToDto(),
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
    public AddressViewModel? Address { get; set; }

    public UiUpdateCarrierDto ToDto(Guid carrierId) => new()
    {
        Id = carrierId,
        FirstName = FirstName,
        LastName = LastName,
        PhoneNumber = PhoneNumber,
        Email = Email,
        CityId = CityId,
        Address = Address?.ToDto()
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

    public DateTime ContractStartDate { get; set; } = DateTime.UtcNow.Date.AddDays(1);

    [Range(1, int.MaxValue)]
    public int ContractYears { get; set; }

    public Guid WarehouseId { get; set; }

    public IReadOnlyCollection<WarehouseOptionViewModel> Warehouses { get; set; } = [];

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
