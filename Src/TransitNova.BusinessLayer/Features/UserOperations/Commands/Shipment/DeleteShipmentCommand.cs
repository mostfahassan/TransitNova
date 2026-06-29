using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment
{
    public record DeleteShipmentCommand(Guid RequestId, Guid ShipmentId, Guid AppUserId)
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;

    
}


