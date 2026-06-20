using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.Roles.Commands
{
    public sealed record DeleteRoleCommand(Guid RequestId, Guid RoleId) : IdempotantCommand<BaseResult>(RequestId);
}
