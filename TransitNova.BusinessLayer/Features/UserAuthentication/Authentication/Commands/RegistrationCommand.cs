using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands
{
    public sealed record RegistrationCommand(Guid RequestId, RegisterDto Dto)
        : IdempotantCommand<Result<AuthResponseDto>>(RequestId);

}
