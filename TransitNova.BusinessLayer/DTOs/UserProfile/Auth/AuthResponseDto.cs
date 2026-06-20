
namespace TransitNova.BusinessLayer.DTOs.UserProfile.Auth
{
    public class AuthResponseDto
    {
        public bool IsAuthenticated { get; set; }
        public Guid Id  { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; 
        public List<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
