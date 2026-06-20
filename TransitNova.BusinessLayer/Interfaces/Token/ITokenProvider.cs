
using TransitNova.BusinessLayer.DTOs.AppUser;
namespace TransitNova.BusinessLayer.Interfaces.Token
{
    public interface ITokenProvider 
    {
            Task<string> GenerateToken(AppUserDto user);
            string GenerateRefreshToken();      
    }
}
