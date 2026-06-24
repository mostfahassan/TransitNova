
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public record CarrierAvailabilityCommand(Guid RequestId, Guid CarrierId)
        : IdempotentCommand<BaseResult>(RequestId);
    
}
