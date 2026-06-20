using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public record CancelShipmentCommand(Guid RequestId, Guid AppUserId, Guid ShipmentId) 
        : IdempotantCommand<BaseResult>(RequestId);
  
}
