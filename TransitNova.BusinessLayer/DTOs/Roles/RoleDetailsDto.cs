
namespace TransitNova.BusinessLayer.DTOs.Roles
{
    public sealed class RoleDetailsDto
    {
        public string RoleId { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public int UsersCount { get; set; }

        public IReadOnlyCollection<RoleUserDto> Users { get; set; } = [];
    }

    public sealed class RoleUserDto
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string UserType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public sealed class RoleSummaryDto
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public sealed record RoleNameDto(string RoleName);

    public sealed class RoleMembersDto
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public int TotalUsers { get; set; }

        public IReadOnlyCollection<RoleMemberDto> Users { get; set; } = [];
    }

    public sealed class RoleMemberDto
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string UserType { get; set; } = string.Empty;

        public bool IsInRole { get; set; }
    }

    public sealed class RoleMemberUpdateDto
    {
        public Guid UserId { get; set; }

        public bool IsInRole { get; set; }
    }

    public sealed class UpdateRoleMembersDto
    {
        public Guid RoleId { get; set; }

        public IReadOnlyCollection<RoleMemberUpdateDto> Users { get; set; } = [];
    }
}
