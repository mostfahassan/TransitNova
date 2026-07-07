using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Carriers.Commands
{
    public sealed record CompleteCarrierTripCommand(Guid RequestId, Guid TripId, Guid CarrierId)
        : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
}