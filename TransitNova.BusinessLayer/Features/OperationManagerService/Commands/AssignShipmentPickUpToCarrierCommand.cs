using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public sealed record AssignShipmentPickUpToCarrierCommand (Guid RequestId, Guid ShipmentId, Guid OperationManagerId, Guid CarrierId) 
        : IdempotantCommand<BaseResult>(RequestId), ITransactional;
}
