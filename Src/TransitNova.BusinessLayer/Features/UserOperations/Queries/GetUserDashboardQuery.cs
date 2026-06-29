using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.UserOperations.Queries
{
    public record GetUserDashboardQuery(Guid AppUserId) : IQuery<Result<ProfileDashboardDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Users.Dashboard(AppUserId);
    }
    
}

