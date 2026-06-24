namespace TransitNova.BusinessLayer.DTOs.UserProfile
{
    public class UserFiltrationDto
    {
        public string? SearchTerm { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
