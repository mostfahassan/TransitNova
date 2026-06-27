using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands
{
    public sealed record UpdateWarehouseCommand(Guid RequestId, Guid WarehouseId, Guid AdminId, UpdateWarehouseDto Dto)
        : IdempotentCommand<BaseResult>(RequestId), ITransactional;
}
