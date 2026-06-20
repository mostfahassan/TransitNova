
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Token.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.Token;
namespace TransitNova.BusinessLayer.Features.Token.Handler
{
    public sealed class RevokeRefreshTokenHandler(IRefreshTokenRepository tokenRepo) : ICommandHandler<RevokeRefreshTokenCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await tokenRepo.RevokeAllUserTokenAsync(request.AppUserId, cancellationToken);
            return BaseResult.Success();
        }
    }
}
