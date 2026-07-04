using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Roles.Queries
{
    public sealed record GetRolesQuery() : IQuery<Result<List<RoleSummaryDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Roles.List;
    };
}
