using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed class UiRoleMembersDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalUsers / (double)PageSize);
    public IReadOnlyCollection<UiRoleMemberDto> Users { get; set; } = [];

    public static UiRoleMembersDto ToUiDto(RoleMembersDto source) =>
        new()
        {
            RoleId = source.RoleId,
            RoleName = source.RoleName,
            TotalUsers = source.TotalUsers,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            Users = source.Users.Select(UiRoleMemberDto.ToUiDto).ToList()
        };
}
