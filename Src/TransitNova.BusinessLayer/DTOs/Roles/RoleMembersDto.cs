namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class RoleMembersDto
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public int TotalUsers { get; set; }

        public IReadOnlyCollection<RoleMemberDto> Users { get; set; } = [];
    }

}
