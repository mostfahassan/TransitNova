using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands
{
    public sealed record CreateWarehouseCommand(Guid RequestId, Guid AdminId, CreateWarehouseDto Dto)
        : IdempotentCommand<Result<WarehouseDto>>(RequestId), ITransactional;
}
