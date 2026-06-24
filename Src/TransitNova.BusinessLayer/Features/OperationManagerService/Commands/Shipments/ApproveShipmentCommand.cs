using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments
{
    public record ApproveShipmentCommand(Guid RequestId, Guid OperationManagerId ,Guid ShipmentId)
        : IdempotentCommand<BaseResult>(RequestId); 
}
