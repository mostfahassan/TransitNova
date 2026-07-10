namespace TransitNova.BusinessLayer.DTOs.Roles
{

    public sealed class RoleMemberDto
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string UserType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public bool IsInRole { get; set; }
    }

}
