
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record CompleteShipmentToWarehouseCommand(Guid RequestId, Guid ShipmentId, Guid CarrierId)
        : IdempotantCommand<BaseResult>(RequestId);
    
}
