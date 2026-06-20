using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public record ApproveShipmentCommand(Guid RequestId, Guid OperationManagerId ,Guid ShipmentId)
        : IdempotantCommand<BaseResult>(RequestId); 
}
