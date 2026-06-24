using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile;

public class UiUserProfileDto
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
    public int TotalShipmentsSent { get; set; }
    public string? BundleName { get; set; }

    public static UiUserProfileDto ToUiDto(UserProfileDto source) =>
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
            TotalShipmentsSent = source.TotalShipmentsSent,
            BundleName = source.BundleName
        };
}
