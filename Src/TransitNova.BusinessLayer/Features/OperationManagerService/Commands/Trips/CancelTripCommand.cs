using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips
{
    public sealed record CancelTripCommand(Guid TripId, Guid OperationManagerId)
        : ICommand<BaseResult>, ITransactional, ICacheInvalidator;
}


