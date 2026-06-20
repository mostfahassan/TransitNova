using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public record DeleteShipmentCommand(Guid RequestId, Guid ShipmentId, Guid AppUserId)
        : IdempotantCommand<BaseResult>(RequestId);

    
}
