using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyCommands
{
    public sealed class UpdateRoleMembersHandler(
        IRolesCommandsService rolesCommandsService,
        ILogger<UpdateRoleMembersHandler> logger)
        : ICommandHandler<UpdateRoleMembersCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateRoleMembersCommand request, CancellationToken ct)
        {
            logger.LogInformation(
                "Updating role members. RoleId: {RoleId}, UsersCount: {UsersCount}",
                request.RoleId,
                request.Users.Count);


            await rolesCommandsService.UpdateRoleMembersAsync(request.RoleId, request.Users, ct);

            logger.LogInformation(
                "Role members updated successfully. RoleId: {RoleId}, UsersCount: {UsersCount}",
                request.RoleId,
                request.Users.Count);

            return BaseResult.Success();
        }

    }
}
