
using TransitNova.BusinessLayer.DTOs.AppUser;
namespace TransitNova.BusinessLayer.Interfaces.Token
{
    public interface ITokenProvider 
    {
            Task<string> GenerateTokenAsync(AppUserDto user);
            string GenerateRefreshToken();      
    }
}
