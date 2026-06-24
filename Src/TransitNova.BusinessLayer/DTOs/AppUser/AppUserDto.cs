using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.AppUser
{
    public class AppUserDto
    {
        public Guid Id { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string? FullName { get; init; }
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public bool EmailConfirmed { get; init; }
        public bool IsLockedOut { get; init; }
        public List<string> Roles { get; set; } = [];
        public UserType UserType { get; init; }
    }
}
