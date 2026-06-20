using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public record GetUserDashboardQuery(Guid AppUserId) : IQuery<Result<ProfileDashboardDto>>, ICachable
    {
        public string CacheKey => CacheKeys.UserDashboard(AppUserId);
    }
    
}
