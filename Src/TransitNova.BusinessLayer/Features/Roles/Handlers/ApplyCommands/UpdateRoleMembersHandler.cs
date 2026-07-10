using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyCommands
{
    public sealed class UpdateRoleMembersHandler(
        IRolesCommandsService rolesCommandsService,
        ILogger<UpdateRoleMembersHandler> logger)
        : ICommandHandler<UpdateRoleMembersCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateRoleMembersCommand request, CancellationToken ct)
        {
            logger.LogInformation("Updating role members. RoleId: {RoleId}, UsersCount: {UsersCount}", request.RoleId, request.Users.Count);

            await rolesCommandsService.UpdateRoleMembersAsync(request.RoleId, request.Users, ct);

            logger.LogInformation("Role members updated successfully. RoleId: {RoleId}, UsersCount: {UsersCount}", request.RoleId, request.Users.Count);


            CacheInvalidationContext.Set(request, CacheKeys.Roles.List, CacheKeys.Roles.Member);
            return BaseResult.Success();
        }

    }
}
