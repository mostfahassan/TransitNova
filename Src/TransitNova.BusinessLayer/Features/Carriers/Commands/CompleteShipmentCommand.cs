
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record CompleteShipmentCommand(Guid RequestId, Guid ShipmentId, Guid CarrierId)
        : IdempotentCommand<BaseResult>(RequestId), ITransactional;
    
}
