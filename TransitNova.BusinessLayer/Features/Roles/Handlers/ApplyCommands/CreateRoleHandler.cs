using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Roles.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
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
            await rolesCommandsService.AddNewRole(request.RoleName, ct);
            logger.LogInformation("Role created successfully. RoleName: {RoleName}", request.RoleName);
            return BaseResult.Created("Role created successfully.");

        }
    }
}
