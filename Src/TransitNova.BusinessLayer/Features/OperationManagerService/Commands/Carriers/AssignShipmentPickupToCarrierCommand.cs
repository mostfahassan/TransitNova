using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers
{
    public sealed record AssignShipmentPickupToCarrierCommand (Guid RequestId, Guid ShipmentId, Guid OperationManagerId, Guid CarrierId) 
        : IdempotentCommand<BaseResult>(RequestId), ITransactional, ICacheInvalidator;
}


