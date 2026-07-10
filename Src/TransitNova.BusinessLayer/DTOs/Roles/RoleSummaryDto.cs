namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class RoleSummaryDto
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public int TotalUsers { get; set; }
    }

}
