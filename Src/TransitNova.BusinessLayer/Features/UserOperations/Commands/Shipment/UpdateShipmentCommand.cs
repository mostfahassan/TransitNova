using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment
{
    public record UpdateShipmentCommand(Guid RequestId, Guid AppUserId, Guid ShipmentId, UpdateShipmentDto Dto) 
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
    
}


