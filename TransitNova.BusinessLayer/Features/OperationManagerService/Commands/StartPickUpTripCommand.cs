using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands
{
    public  record  StartPickUpTripCommand(Guid RequestId, Guid OperationManagerId ,Guid CarrierId)
       :IdempotantCommand<BaseResult>(RequestId), ITransactional;

}
