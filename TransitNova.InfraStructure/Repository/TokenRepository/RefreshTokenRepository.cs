
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.TokenRepository
{
    internal sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        public async Task AddRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken ct)
        {
            var refreshDto = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(3)
            };
     
            await context.RefreshTokens.AddAsync(refreshDto, ct);
        }

        public async Task<RefreshTokenDto?> GetRefreshTokenAsync(string refreshToken, CancellationToken ct)
           => await context.RefreshTokens.AsNoTracking()
                .Where(rt => rt.Token == refreshToken)
                .Select(rt => new RefreshTokenDto
                {
                    Token = rt.Token,
                    UserId = rt.UserId,
                    User = rt.User != null ? new AppUserDto
                    {
                        UserType = rt.User.UserType,
                    } : null,
                    ExpiresOn = rt.ExpiresAt,
                    IsRevoked = rt.IsRevoked,

                }).FirstOrDefaultAsync(ct);

        public async Task<int> RevokeAllUserTokenAsync(Guid userId, CancellationToken ct)
            => await context.RefreshTokens.Where(rt => rt.UserId == userId)
             .ExecuteDeleteAsync(ct);
       

        public async Task<int> RevokeOldRefreshTokenAsync(Guid userId, string oldToken, string newToken, CancellationToken ct)
             => await context.RefreshTokens.Where(rt => rt.UserId == userId && rt.Token == oldToken)
                 .ExecuteUpdateAsync(p => p
                  .SetProperty(r => r.RevokedAt, DateTime.UtcNow)
                  .SetProperty(r => r.IsRevoked, true)
                  .SetProperty(r => r.ReplacedByToken, newToken), ct);
               
    }
}
