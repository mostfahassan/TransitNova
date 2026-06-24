using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNovaUI.BusinessLayer.DTOs.Roles;

public sealed class UiUpdateRoleMembersDto
{
    public IReadOnlyCollection<UiRoleMemberUpdateDto> Users { get; set; } = [];

    public static UpdateRoleMembersDto ToDto(UiUpdateRoleMembersDto source) =>
        new()
        {
            Users = source.Users.Select(UiRoleMemberUpdateDto.ToDto).ToList()
        };

}
