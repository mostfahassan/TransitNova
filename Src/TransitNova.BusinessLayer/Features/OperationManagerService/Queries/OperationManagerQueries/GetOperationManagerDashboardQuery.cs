using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries
{
    public sealed record GetOperationManagerDashboardQuery(Guid OperationManagerId) : IQuery<Result<OperationManagerDashboardDto>>, ICachable
    {
        public string CacheKey => CacheKeys.OperationManagers.Dashboard(OperationManagerId);
    }
   
}

