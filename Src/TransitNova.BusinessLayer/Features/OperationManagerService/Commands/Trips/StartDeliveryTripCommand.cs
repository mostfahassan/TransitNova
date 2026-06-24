using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips
{
    public record  StartDeliveryTripCommand(Guid RequestId, Guid OperationManagerId ,Guid CarrierId)
       :IdempotentCommand<BaseResult>(RequestId), ITransactional;

}
