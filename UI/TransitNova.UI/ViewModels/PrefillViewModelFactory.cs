using TransitNovaUI.BusinessLayer.DTOs.Bundle;
using TransitNovaUI.BusinessLayer.DTOs.Carrier;
using TransitNovaUI.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.DTOs.Country;
using TransitNovaUI.BusinessLayer.DTOs.Roles;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Warehouse;
using TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

namespace TransitNova.UI.ViewModels;

public static class PrefillViewModelFactory
{
    public static BundleFormViewModel Bundle(Guid bundleId, UiRetrieveBundleDto source) => new()
    {
        BundleId = bundleId,
        BundleName = source.BundleName,
        TotalWeight = source.TotalWeight,
        BundlePrice = source.BundlePrice,
        BundleDescription = source.BundleDescription,
        TotalDistance = source.TotalDistance,
        TotalShipments = source.TotalShipments
    };

    public static CityFormViewModel City(UiCityDto source) => new()
    {
        Name = source.Name,
        GovernmentId = source.GovernmentId
    };

    public static GovernmentFormViewModel Government(UiGovernmentDto source) => new()
    {
        Name = source.Name
    };

    public static RoleFormViewModel Role(UiRoleSummaryDto source) => new()
    {
        RoleName = source.RoleName ?? string.Empty
    };

    public static WarehouseFormViewModel Warehouse(UiWarehouseDto source) => new()
    {
        Name = source.Name,
        Type = source.Type,
        Address = source.Address,
        Capacity = source.Capacity,
        CurrentUsage = source.CurrentUsage,
        OperatingHours = source.OperatingHours,
        ZoneIds = source.ZoneIds
    };

    public static UpdateShipmentViewModel Shipment(UiRetrieveShipmentDto source) => new()
    {
        ReceiverId = source.ReceiverId,
        DeliveryAddress = source.DeliveryAddress,
        PickupAddress = source.PickupAddress,
        PackageSpecification = new PackageSpecificationViewModel
        {
            Weight = source.PackageSpecification.Weight,
            Width = source.PackageSpecification.Width,
            Height = source.PackageSpecification.Height,
            Length = source.PackageSpecification.Length
        },
        ShipmentType = source.ShipmentType,
        TransportationMode = source.TransportationMode
    };

    public static CarrierProfileFormViewModel CarrierProfile(UiCarrierProfileDto source)
    {
        var names = SplitFullName(source.FullName);

        return new CarrierProfileFormViewModel
        {
            Id = source.Id,
            FirstName = names.FirstName,
            LastName = names.LastName,
            PhoneNumber = source.PhoneNumber,
            Email = source.Email,
            Address = source.Address
        };
    }

    public static WarehouseManagerProfileFormViewModel WarehouseManagerProfile(UiWarehouseManagerDashboardDto source)
    {
        var names = SplitFullName(source.Manager.ManagerName);

        return new WarehouseManagerProfileFormViewModel
        {
            Id = source.Manager.ManagerId,
            FirstName = names.FirstName,
            LastName = names.LastName,
            WarehouseId = source.Manager.WarehouseId
        };
    }

    private static (string? FirstName, string? LastName) SplitFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (null, null);

        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 1 ? (parts[0], null) : (parts[0], parts[1]);
    }
}