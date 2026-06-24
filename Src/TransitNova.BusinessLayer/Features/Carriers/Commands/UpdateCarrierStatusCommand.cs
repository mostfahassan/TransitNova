
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record UpdateCarrierStatusCommand(Guid RequestId, Guid CarrierId, CarrierStatus Status)
        : IdempotentCommand<BaseResult>(RequestId);
    
}
