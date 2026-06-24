using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public record RateDeliveryCarrierCommand(Guid RequestId, Guid AppUserId, Guid shipmentId, RatingCarrierDto Dto) 
        : IdempotentCommand<BaseResult>(RequestId), ITransactional;

}
   
