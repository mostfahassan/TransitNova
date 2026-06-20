using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public  record  StartPickUpTripCommand(Guid RequestId, Guid OperationManagerId ,Guid CarrierId)
       :IdempotantCommand<BaseResult>(RequestId);

}
