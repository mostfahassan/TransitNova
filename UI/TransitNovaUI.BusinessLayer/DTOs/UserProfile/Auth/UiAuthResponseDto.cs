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
