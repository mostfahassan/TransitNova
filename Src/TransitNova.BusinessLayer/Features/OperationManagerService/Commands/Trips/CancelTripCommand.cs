using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips
{
    public sealed record CancelTripCommand(Guid TripId, Guid OperationManagerId)
        : ICommand<BaseResult>, ITransactional;
}
