using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments
{
    public sealed record RejectShipmentCommand(Guid RequestId, Guid OperationManagerId, Guid ShipmentId, string RejectionReason) 
    : IdempotentCommand<BaseResult>(RequestId);
}
