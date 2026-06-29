using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment
{
    public record IssueShipmentCommand(Guid RequestId, Guid AppUserId, Guid ShipmentId ,string IssueMessage) 
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
  
}


