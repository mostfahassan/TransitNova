using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
namespace TransitNova.BusinessLayer.Features.Warehouses.Commands
{
    public sealed record CreateWarehouseCommand(Guid RequestId, Guid AdminId, CreateWarehouseDto Dto)
        : IdempotentCommand<Result<WarehouseDto>>(RequestId),ICacheInvalidator;
}
