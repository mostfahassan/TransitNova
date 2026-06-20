
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.Features.Token.Handler;
using TransitNova.BusinessLayer.Interfaces.Repositories.Token;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.TokenServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.Domain.DomainExceptions;
using static TransitNova.BusinessLayer.Common.Exceptions.ReusedRefreshTokenException;
namespace TransitNova.BusinessLayer.Services.TokenServices
{
    internal class TokenService(IRefreshTokenRepository tokenRepo,
        ILogger<GenerateRefreshTokenHandler> logger,
        ITokenProvider tokenProvider ,
        IUserAuthQueryService user,
        IUnitOfWork unitOfWork) : ITokenService
    {
        public async Task<GeneratedTokenResult> GetNewTokenAsync(string OldrefreshToken, CancellationToken cancellationToken)
        {
            //=== Get User Refresh Token
            var token = await tokenRepo.GetRefreshTokenAsync(OldrefreshToken, cancellationToken);
            var validatedToken = TokenValidator(token);

            // === Check If Refresh OldrefreshToken reused
            if (validatedToken.IsRevoked)
            {
                logger.LogCritical("Refresh OldrefreshToken reuse detected. UserId: {UserId}, Token: {Token}", validatedToken.UserId, validatedToken.Token);
                await tokenRepo.RevokeAllUserTokenAsync(validatedToken.UserId, cancellationToken);
                throw new ReusedRefreshTokenException(validatedToken.UserId, validatedToken.Token);
            }

            //=== Generate Refresh Token 
            var refreshToken = tokenProvider.GenerateRefreshToken();

            var affectedRows = await tokenRepo.RevokeOldRefreshTokenAsync(validatedToken.UserId, validatedToken.Token, refreshToken, cancellationToken);
            if (affectedRows <= 0) throw new RevokingRefreshTokenException();

            await tokenRepo.AddRefreshTokenAsync(validatedToken.UserId, refreshToken, cancellationToken);
            //======= Attempts To Save Changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            //=== Getting User Roles 
            var roles = await user.GetUserRolesAsync(validatedToken.UserId, cancellationToken);
            validatedToken.User!.Roles = [.. roles];


            //=== Generate New Access Token
            var accessToken = await tokenProvider.GenerateToken(validatedToken.User);

            var GeneratedTokenResult = new GeneratedTokenResult
            {
                ValidToken = validatedToken,
                AccessToken = accessToken,
                RefreshToken= refreshToken,
                Roles = [..roles],
            };

            return GeneratedTokenResult;

        }
        static RefreshTokenDto TokenValidator(RefreshTokenDto? token)
        {
            //=== Check Token
            if (token == null) throw new RefreshTokenNotFoundException();

            // === Check Token Expiration Date
            if (token.ExpiresOn < DateTime.UtcNow) throw new InvalidRefreshTokenException();
            
            //=== Check User Existence
            if (token.User is null) throw new EntityNotFoundException("User Not Found", "USER_NOT_FOUND");
            return token;
        }
    }
    public class GeneratedTokenResult
    {
        public RefreshTokenDto ValidToken { get; init; } = null!;
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public List<string> Roles { get; init; } = [];
    }
}
