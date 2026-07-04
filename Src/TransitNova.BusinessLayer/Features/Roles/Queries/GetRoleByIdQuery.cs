using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Roles.Queries
{
    public sealed record GetRoleByIdQuery(Guid RoleId) : IQuery<Result<RoleSummaryDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Roles.ById(RoleId);
    };
}
