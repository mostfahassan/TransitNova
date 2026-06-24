
using TransitNova.BusinessLayer.Services.TokenServices;
namespace TransitNova.BusinessLayer.Interfaces.Services.TokenServices
{
    public interface ITokenService
    {
        Task<GeneratedTokenResult> GetNewTokenAsync(string token,CancellationToken cancellationToken);
    }
}
