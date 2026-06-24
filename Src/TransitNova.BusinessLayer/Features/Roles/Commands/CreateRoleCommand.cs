using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Roles.Commands
{
    public sealed record CreateRoleCommand(Guid RequestId, string RoleName) : IdempotentCommand<BaseResult>(RequestId);
}
