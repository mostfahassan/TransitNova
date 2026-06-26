using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment
{
    public record CancelShipmentCommand(Guid RequestId, Guid AppUserId, Guid ShipmentId) 
        : IdempotentCommand<BaseResult>(RequestId);
  
}
