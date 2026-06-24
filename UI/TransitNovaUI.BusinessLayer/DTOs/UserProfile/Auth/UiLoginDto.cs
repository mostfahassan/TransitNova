using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.Domain.Enums.Users;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

public sealed record UiLoginDto(string Password, string Email)
{
    public static LoginDto ToDto(UiLoginDto source) =>
        new(source.Password, source.Email);

}
