using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands
{
    public sealed record LoginCommand(LoginDto Dto) :ICommand<Result<AuthResponseDto>>;


}
