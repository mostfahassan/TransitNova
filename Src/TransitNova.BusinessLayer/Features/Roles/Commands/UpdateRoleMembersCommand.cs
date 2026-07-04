using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;

namespace TransitNova.BusinessLayer.Features.Roles.Commands
{
    public sealed record UpdateRoleMembersCommand(Guid RequestId, Guid RoleId, IReadOnlyCollection<RoleMemberUpdateDto> Users)
        : IdempotentCommand<BaseResult>(RequestId),ICacheInvalidator;
}




