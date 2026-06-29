
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Common.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record DeleteCarrierCommand(Guid RequestId, Guid CarrierId,Guid AdminId) 
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
}



