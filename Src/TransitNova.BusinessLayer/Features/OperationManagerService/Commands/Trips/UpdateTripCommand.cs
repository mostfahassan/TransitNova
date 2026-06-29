using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips
{
    public sealed record UpdateTripCommand(Guid TripId, Guid OperationManagerId, UpdateTripDto Dto)
        : ICommand<BaseResult>, ITransactional, ICacheInvalidator;
}


