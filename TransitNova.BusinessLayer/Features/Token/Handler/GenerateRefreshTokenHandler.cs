using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.Token.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.TokenServices;
namespace TransitNova.BusinessLayer.Features.Token.Handler
{
    public sealed class GenerateRefreshTokenHandler(
        ITokenService tokenService,
        ILogger<GenerateRefreshTokenHandler> logger) : ICommandHandler<GenerateRefreshTokenCommand, Result<AuthResponseDto>>
    {
        public async Task<Result<AuthResponseDto>> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            
            var TokenResult = await tokenService.GetNewTokenAsync(request.RefreshToken, cancellationToken);
            //=== BuildAsync Response
            var authResponseBuilder = new AuthResponseDto
            {
                IsAuthenticated = true,
                Id = TokenResult.ValidToken.UserId,
                Token = TokenResult.AccessToken,
                RefreshToken = TokenResult.RefreshToken,
                UserType = TokenResult.ValidToken.User!.UserType.ToString(),
                Roles = TokenResult.Roles
            };

            logger.LogInformation("Access token refreshed successfully for User {UserId}", TokenResult.ValidToken.UserId);
            return Result<AuthResponseDto>.Success(authResponseBuilder);
        }
      
    }
}
