using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public sealed record FilterUsersQuery(UserFiltrationDto FilterCriteria)
        : IQuery<Result<PagedResult<AdminUserDetailsDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.UserFilter(FilterCriteria);
    }
}
