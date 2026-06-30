
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record CompleteShipmentCommand(Guid RequestId, Guid ShipmentId, Guid CarrierId)
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
    
}



