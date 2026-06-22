using TransitNova.BusinessLayer.DTOs.Roles;

namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed class UiRoleSummaryDto
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }

    public static UiRoleSummaryDto ToUiDto(RoleSummaryDto source) =>
        new() { RoleId = source.RoleId, RoleName = source.RoleName };
}

public sealed record UiRoleNameDto(string RoleName)
{
    public static RoleNameDto ToDto(UiRoleNameDto source) => new(source.RoleName);

}

public sealed class UiRoleMemberDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool IsInRole { get; set; }

    public static UiRoleMemberDto ToUiDto(RoleMemberDto source) =>
        new()
        {
            UserId = source.UserId,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            UserType = source.UserType,
            IsInRole = source.IsInRole
        };
}

public sealed class UiRoleMembersDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public IReadOnlyCollection<UiRoleMemberDto> Users { get; set; } = [];

    public static UiRoleMembersDto ToUiDto(RoleMembersDto source) =>
        new()
        {
            RoleId = source.RoleId,
            RoleName = source.RoleName,
            TotalUsers = source.TotalUsers,
            Users = source.Users.Select(UiRoleMemberDto.ToUiDto).ToList()
        };
}

public sealed class UiRoleMemberUpdateDto
{
    public Guid UserId { get; set; }
    public bool IsInRole { get; set; }

    public static RoleMemberUpdateDto ToDto(UiRoleMemberUpdateDto source) =>
        new() { UserId = source.UserId, IsInRole = source.IsInRole };

}

public sealed class UiUpdateRoleMembersDto
{
    public IReadOnlyCollection<UiRoleMemberUpdateDto> Users { get; set; } = [];

    public static UpdateRoleMembersDto ToDto(UiUpdateRoleMembersDto source) =>
        new()
        {
            Users = source.Users.Select(UiRoleMemberUpdateDto.ToDto).ToList()
        };

}
