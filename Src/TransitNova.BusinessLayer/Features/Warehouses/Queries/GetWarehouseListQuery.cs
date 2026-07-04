using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Warehouses.Queries
{
    public sealed record GetWarehouseListQuery()
        : IQuery<Result<List<WarehouseDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Warehouse.List;
    }
}
