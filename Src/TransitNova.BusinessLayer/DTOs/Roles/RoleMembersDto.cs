namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class RoleMembersDto
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public int TotalUsers { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalUsers / (double)PageSize);

        public IReadOnlyCollection<RoleMemberDto> Users { get; set; } = [];
    }

}
