
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Handlers.ApplyingCommands
{
    public sealed class ChangePasswordHandler(
        ILogger<ChangePasswordHandler> logger ,
        IUserAuthCommandsService authRepository) : ICommandHandler<ChangePasswordCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = request.AppUserId;

            await authRepository.ChangePasswordAsync(userId.ToString(), request.Dto.CurrentPassword, request.Dto.NewPassword, cancellationToken);

            logger.LogInformation("Password changed successfully for user {UserId}", userId);
            return BaseResult.Success();
        }
    }
}
