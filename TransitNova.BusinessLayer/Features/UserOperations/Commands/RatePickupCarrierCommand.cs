using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public record RatePickupCarrierCommand(Guid RequestId, Guid AppUserId, Guid shipmentId, RatingCarrierDto Dto)
       : IdempotantCommand<BaseResult>(RequestId);

}
   
