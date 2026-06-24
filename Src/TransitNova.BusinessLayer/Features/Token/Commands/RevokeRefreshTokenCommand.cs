using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Token.Commands
{
    public sealed record RevokeRefreshTokenCommand(Guid RequestId, Guid AppUserId)
     : IdempotentCommand<BaseResult>(RequestId);

    
}
