
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;

using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries
{
    public sealed record GetOperationManagerDashboardQuery : IQuery<Result<OperationManagerDashboardDto>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagerDashboard();
    }
   
}
