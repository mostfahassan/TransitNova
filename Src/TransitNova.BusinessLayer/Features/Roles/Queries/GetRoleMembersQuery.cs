using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;

namespace TransitNova.BusinessLayer.Features.Roles.Queries
{
    public sealed record GetRoleMembersQuery(Guid RoleId) : IQuery<Result<RoleMembersDto>>;
}
