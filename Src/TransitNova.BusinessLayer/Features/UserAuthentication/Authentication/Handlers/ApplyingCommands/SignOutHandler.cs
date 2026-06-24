using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands
{
    public sealed class SignOutHandler(ILogger<SignOutHandler> logger, IUserAuthRulesService userRules)
 : ICommandHandler<SignOutCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(SignOutCommand request, CancellationToken cancellationToken)
        {
            await userRules.SignOutAsync(cancellationToken);
            logger.LogInformation("User Signed Out Successfully at {time}", DateTime.UtcNow);
            return BaseResult.Success();
        }
    }
}
