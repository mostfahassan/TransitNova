using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands
{
    public sealed record RegistrationCommand(RegisterDto Dto)
        :ICommand<Result<AuthResponseDto>>, ITransactional;

}
