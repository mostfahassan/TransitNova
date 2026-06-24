namespace TransitNova.BusinessLayer.DTOs.UserProfile
{
    public class AdminUserDetailsDto : UserProfileDto
    {
        public Guid UserId { get; set; }
        public Guid ProfileId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
