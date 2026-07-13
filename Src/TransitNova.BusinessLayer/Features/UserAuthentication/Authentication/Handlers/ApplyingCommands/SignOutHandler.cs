using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;

namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands;

public sealed class SignOutHandler(
    ILogger<SignOutHandler> logger,
    IRefreshTokenRepository tokenRepository)
    : ICommandHandler<SignOutCommand, BaseResult>
{
    public async Task<BaseResult> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        var revokedTokenCount = await tokenRepository.RevokeAllUserTokenAsync(
            request.UserId,
            cancellationToken);

        logger.LogInformation(
            "User {UserId} signed out successfully. Revoked refresh tokens: {RevokedTokenCount}",
            request.UserId,
            revokedTokenCount);

        return BaseResult.Success();
    }
}
