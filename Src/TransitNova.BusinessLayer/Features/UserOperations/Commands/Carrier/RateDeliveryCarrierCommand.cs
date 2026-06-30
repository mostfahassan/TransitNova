using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Carrier
{
    public record RateDeliveryCarrierCommand(Guid RequestId, Guid AppUserId, Guid ShipmentId, RatingCarrierDto Dto) 
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;

}
   


