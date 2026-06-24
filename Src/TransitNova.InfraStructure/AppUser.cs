
using Microsoft.AspNetCore.Identity;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.InfraStructure
{
    public class AppUser : IdentityUser<Guid>
    {
        private readonly List<RefreshToken> _refreshTokens = new();
        public UserType UserType { get; set; }
        public string?  FullName { get; set; }
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;
    }
}
