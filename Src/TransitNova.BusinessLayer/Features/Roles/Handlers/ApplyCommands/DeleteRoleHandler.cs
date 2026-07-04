using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyCommands
{
    public sealed class DeleteRoleHandler(
        IRolesCommandsService rolesCommandsService,
        ILogger<DeleteRoleHandler> logger)
        : ICommandHandler<DeleteRoleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteRoleCommand request, CancellationToken ct)
        {
            logger.LogInformation("Deleting role. RoleId: {RoleId}", request.RoleId);


            await rolesCommandsService.DeleteRoleAsync(request.RoleId, ct);

            logger.LogInformation("Role deleted successfully. RoleId: {RoleId}", request.RoleId);
            CacheInvalidationContext.Set(request, CacheKeys.Roles.List, CacheKeys.Roles.MemberList);
            return BaseResult.Success();

        }
    }
}
