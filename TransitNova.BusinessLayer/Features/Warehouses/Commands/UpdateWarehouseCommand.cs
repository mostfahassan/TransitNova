using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands
{
    public sealed record UpdateWarehouseCommand(Guid RequestId, Guid WarehouseId, Guid AdminId, UpdateWarehouseDto Dto)
        : IdempotantCommand<BaseResult>(RequestId);
}
