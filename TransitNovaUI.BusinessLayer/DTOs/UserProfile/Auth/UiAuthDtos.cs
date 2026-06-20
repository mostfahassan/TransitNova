using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.Domain.Enums.Users;

namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile.Auth;

public sealed class UiAuthResponseDto
{
    public bool IsAuthenticated { get; set; }
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public string Token { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;

    public static UiAuthResponseDto ToUiDto(AuthResponseDto source) =>
        new()
        {
            IsAuthenticated = source.IsAuthenticated,
            Id = source.Id,
            Username = source.Username,
            Email = source.Email,
            Roles = [.. source.Roles],
            Token = source.Token,
            UserType = source.UserType,
            RefreshToken = source.RefreshToken
        };
}

public sealed record UiChangePasswordDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword)
{
    public static ChangePasswordDto ToDto(UiChangePasswordDto source) =>
        new(source.CurrentPassword, source.NewPassword, source.ConfirmNewPassword);

}

public sealed record UiLoginDto(string Password, string Email)
{
    public static LoginDto ToDto(UiLoginDto source) =>
        new(source.Password, source.Email);

}

public sealed class UiRegisterDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
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
            Address = source.Address,
            UserType = source.UserType,
            CityId = source.CityId
        };

}
