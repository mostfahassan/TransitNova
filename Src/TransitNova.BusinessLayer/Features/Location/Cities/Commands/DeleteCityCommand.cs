using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Commands
{
    public sealed record DeleteCityCommand(Guid RequestId, int Id) : IdempotentCommand<BaseResult>(RequestId), ICacheInvalidator;
}


