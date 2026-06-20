using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public sealed record RejectShipmentCommand(Guid RequestId, Guid OperationManagerId, Guid ShipmentId, string RejectionReason) : IdempotantCommand<BaseResult>(RequestId);
}
