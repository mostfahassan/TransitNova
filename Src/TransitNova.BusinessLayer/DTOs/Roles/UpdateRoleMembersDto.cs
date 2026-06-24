namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class UpdateRoleMembersDto
    {
        public IReadOnlyCollection<RoleMemberUpdateDto> Users { get; set; } = [];
    }

}
