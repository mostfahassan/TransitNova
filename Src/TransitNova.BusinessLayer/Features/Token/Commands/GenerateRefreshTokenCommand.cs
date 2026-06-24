using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;

namespace TransitNova.BusinessLayer.Features.Token.Commands
{
    public sealed record GenerateRefreshTokenCommand(Guid RequestId, string RefreshToken) 
    : IdempotentCommand<Result<AuthResponseDto>>(RequestId);

    
}
