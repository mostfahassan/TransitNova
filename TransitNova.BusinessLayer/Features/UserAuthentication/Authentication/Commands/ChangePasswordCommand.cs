using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands
{
    public sealed record ChangePasswordCommand(Guid RequestId, ChangePasswordDto Dto,Guid AppUserId)
        : IdempotantCommand<BaseResult>(RequestId);
  
}
