using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed class UiRoleMemberUpdateDto
{
    public Guid UserId { get; set; }
    public bool IsInRole { get; set; }

    public static RoleMemberUpdateDto ToDto(UiRoleMemberUpdateDto source) =>
        new() { UserId = source.UserId, IsInRole = source.IsInRole };

}
