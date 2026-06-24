namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class RoleUserDto
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string UserType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

}
