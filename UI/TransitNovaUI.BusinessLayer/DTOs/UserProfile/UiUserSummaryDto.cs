using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.Common.CommonData;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile;

public sealed class UiUserSummaryDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UiAddressDto Address { get; set; } = new();
    public string CityName { get; set; } = string.Empty;

    public static UiUserSummaryDto ToUiDto(UserSummaryDto source) =>
        new()
        {
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = UiAddressDto.ToUiDto(source.Address),
            CityName = source.CityName
        };
}
