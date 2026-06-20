using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public sealed record GetAdminUserDetailsQuery(Guid UserId)
        : IQuery<Result<AdminUserDetailsDto>>, ICachable
    {
        public string CacheKey => CacheKeys.AdminUserDetails(UserId);
    }
}
