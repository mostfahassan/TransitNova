using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed class UiRoleSummaryDto
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public int TotalUsers { get; set; } = 0;

    public static UiRoleSummaryDto ToUiDto(RoleSummaryDto source) =>
        new() { RoleId = source.RoleId, RoleName = source.RoleName, TotalUsers = source.TotalUsers };
}
