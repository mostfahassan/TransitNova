using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.CommonData;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

public sealed class UiRegisterDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UiAddressDto Address { get; set; } = new();
    public UserType UserType { get; set; }
    public int CityId { get; set; }

    public static RegisterDto ToDto(UiRegisterDto source) =>
        new()
        {
            UserName = source.UserName,
            Email = source.Email,
            Password = source.Password,
            ConfirmPassword = source.ConfirmPassword,
            PhoneNumber = source.PhoneNumber,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Address = UiAddressDto.ToDto(source.Address),
            UserType = source.UserType,
            CityId = source.CityId
        };

}
