using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.Domain.Enums.Users;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

public sealed record UiChangePasswordDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword)
{
    public static ChangePasswordDto ToDto(UiChangePasswordDto source) =>
        new(source.CurrentPassword, source.NewPassword, source.ConfirmNewPassword);

}
