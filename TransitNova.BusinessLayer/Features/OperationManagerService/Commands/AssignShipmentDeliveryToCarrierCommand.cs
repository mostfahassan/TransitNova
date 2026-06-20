using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public sealed record AssignShipmentDeliveryToCarrierCommand (Guid RequestId, Guid ShipmentId, Guid OperationManagerId,Guid CarrierId) : IdempotantCommand<BaseResult>(RequestId);

}
