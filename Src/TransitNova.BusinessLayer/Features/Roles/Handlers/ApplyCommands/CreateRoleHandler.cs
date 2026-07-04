using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyCommands
{
    public sealed class CreateRoleHandler(
        IRolesCommandsService rolesCommandsService,
        ILogger<CreateRoleHandler> logger)
        : ICommandHandler<CreateRoleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CreateRoleCommand request, CancellationToken ct)
        {
            logger.LogInformation("Creating role. RoleName: {RoleName}", request.RoleName);
            await rolesCommandsService.AddNewRoleAsync(request.RoleName, ct);
            logger.LogInformation("Role created successfully. RoleName: {RoleName}", request.RoleName);
            CacheInvalidationContext.Set(request, CacheKeys.Roles.List, CacheKeys.Roles.MemberList);
            return BaseResult.Created("Role created successfully.");

        }
    }
}
