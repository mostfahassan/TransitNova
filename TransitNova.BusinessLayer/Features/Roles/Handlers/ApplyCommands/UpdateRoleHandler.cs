using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyCommands
{
    public sealed class UpdateRoleHandler(
        IRolesCommandsService rolesCommandsService,
        ILogger<UpdateRoleHandler> logger)
        : ICommandHandler<UpdateRoleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateRoleCommand request, CancellationToken ct)
        {
            logger.LogInformation("Updating role. RoleId: {RoleId}", request.RoleId);

            await rolesCommandsService.UpdateRole(request.RoleId, request.RoleName, ct);
            logger.LogInformation("Role updated successfully. RoleId: {RoleId}", request.RoleId);
            return BaseResult.Success();

        }
    }
}
