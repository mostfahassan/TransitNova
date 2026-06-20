
using TransitNova.BusinessLayer.DTOs.RefreshToken;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.Token
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenDto?> GetRefreshTokenAsync(string refreshToken, CancellationToken ct);
        Task AddRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct);
        Task<int> RevokeOldRefreshTokenAsync(Guid userId,string oldToken , string newToken, CancellationToken ct);
        Task<int> RevokeAllUserTokenAsync(Guid userId, CancellationToken ct);
    }
}
