using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands
{
    public sealed record SignOutCommand() : ICommand<BaseResult>;
}
