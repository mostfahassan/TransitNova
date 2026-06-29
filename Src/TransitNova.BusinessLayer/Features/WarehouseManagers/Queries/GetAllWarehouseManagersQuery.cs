using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.WarehouseManager;

namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Queries
{
    public sealed record GetAllWarehouseManagersQuery(WarehouseManagerFilterDto Filter)
        : IQuery<Result<PagedResult<WarehouseManagerListDto>>>;
}
