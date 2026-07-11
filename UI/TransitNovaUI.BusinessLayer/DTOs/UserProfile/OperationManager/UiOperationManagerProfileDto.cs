using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.Domain.Enums.Users;

using TransitNovaUI.BusinessLayer.Common.CommonData;

namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile.OperationManager;

public sealed class UiOperationManagerProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UiAddressDto Address { get; set; } = new();
    public UserType UserType { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string GovernmentName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int TotalShipmentHandled { get; set; }
    public int TotalCarriertHandled { get; set; }

    public static UiOperationManagerProfileDto ToUiDto(OperationManagerProfileDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = UiAddressDto.ToUiDto(source.Address),
            UserType = source.UserType,
            CityName = source.CityName,
            GovernmentName = source.GovernmentName,
            CountryName = source.CountryName,
            TotalShipmentHandled = source.TotalShipmentHandled,
            TotalCarriertHandled = source.TotalCarriertHandled
        };
}
