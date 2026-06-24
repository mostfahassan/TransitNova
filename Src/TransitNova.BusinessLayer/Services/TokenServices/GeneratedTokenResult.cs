using TransitNova.BusinessLayer.DTOs.RefreshToken;
namespace TransitNova.BusinessLayer.Services.TokenServices
{
    public class GeneratedTokenResult
    {
        public RefreshTokenDto ValidToken { get; init; } = null!;
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public List<string> Roles { get; init; } = [];
    }

}
