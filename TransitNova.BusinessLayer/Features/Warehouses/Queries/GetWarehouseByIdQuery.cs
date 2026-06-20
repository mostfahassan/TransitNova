using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;

namespace TransitNova.BusinessLayer.Features.Warehouses.Queries
{
    public sealed record GetWarehouseByIdQuery(Guid WarehouseId)
        : IQuery<Result<WarehouseDto?>>;
}
