namespace TransitNova.BusinessLayer.DTOs.Roles
{
    public sealed class RoleDetailsDto
    {
        public string RoleId { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public int UsersCount { get; set; }

        public IReadOnlyCollection<RoleUserDto> Users { get; set; } = [];
    }

}
