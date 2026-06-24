using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Roles.Commands
{
    public sealed record UpdateRoleCommand(Guid RequestId, Guid RoleId, string RoleName) : IdempotentCommand<BaseResult>(RequestId);
}
