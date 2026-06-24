using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed record UiRoleNameDto(string RoleName)
{
    public static RoleNameDto ToDto(UiRoleNameDto source) => new(source.RoleName);

}
