using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands
{
    public sealed record DeleteWarehouseCommand(Guid RequestId, Guid WarehouseId, Guid AdminId)
        : IdempotantCommand<BaseResult>(RequestId);
}
