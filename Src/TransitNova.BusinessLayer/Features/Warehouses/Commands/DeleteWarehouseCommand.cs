using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Warehouses.Commands
{
    public sealed record DeleteWarehouseCommand(Guid RequestId, Guid WarehouseId, Guid AdminId)
        : IdempotentCommand<BaseResult>(RequestId), ITransactional;
}
