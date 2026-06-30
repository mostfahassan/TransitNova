using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers
{
    public sealed record AssignShipmentDeliveryToCarrierCommand (Guid RequestId, Guid ShipmentId, Guid OperationManagerId,Guid CarrierId) :
        IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;

}


