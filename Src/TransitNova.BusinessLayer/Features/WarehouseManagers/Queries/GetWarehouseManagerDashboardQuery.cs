using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Queries
{
    public sealed record GetWarehouseManagerDashboardQuery(Guid ManagerId)
        : IQuery<Result<WarehouseManagerDashboardDto>>, ICachable
    {
        public string CacheKey => $"warehouse-managers:dashboard:manager-id:{ManagerId}";
    }
}
