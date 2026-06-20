
using TransitNova.BusinessLayer.DTOs.AppUser;
namespace TransitNova.BusinessLayer.DTOs.RefreshToken
{
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public AppUserDto? User { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsRevoked { get; set; }
    }
    public sealed record RefreshToken(string Token);
}
