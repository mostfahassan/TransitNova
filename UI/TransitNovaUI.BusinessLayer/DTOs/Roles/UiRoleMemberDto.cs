using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed class UiRoleMemberDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsInRole { get; set; }

    public static UiRoleMemberDto ToUiDto(RoleMemberDto source) =>
        new()
        {
            UserId = source.UserId,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            UserType = source.UserType,
            Status = source.Status,
            IsInRole = source.IsInRole
        };
}
