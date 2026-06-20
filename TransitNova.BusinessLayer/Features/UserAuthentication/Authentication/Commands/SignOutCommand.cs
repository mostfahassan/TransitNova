using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands
{
    public sealed record SignOutCommand(Guid RequestId) : IdempotantCommand<BaseResult>(RequestId);
}
