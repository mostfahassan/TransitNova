using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

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
