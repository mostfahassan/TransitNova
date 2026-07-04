using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Trips
{
    public sealed class CancelTripHandler(
        ITripCommandRepository tripRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelTripHandler> logger)
        : ICommandHandler<CancelTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripForCommandsAsync(request.TripId, cancellationToken);
            if (trip is null) return BaseResult.NotFound(Errors.TripNotFound($"Trip With Id => {request.TripId} Not Found"));

            trip.Cancel(request.OperationManagerId);
       
            await unitOfWork.SaveChangesAsync(cancellationToken);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Carriers.Trips(trip.CarrierId),
                CacheKeys.Carriers.TripDetails(trip.CarrierId, trip.Id),
                CacheKeys.Carriers.Dashboard(trip.CarrierId),
                CacheKeys.Trips.Filter(new FilterTripsDto()),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.Trips.Details(trip.Id));

            logger.LogInformation("Trip cancelled successfully. TripId: {TripId}, OperationManagerId: {OperationManagerId}", trip.Id, request.OperationManagerId);
            return BaseResult.Success();
        }
    }
}


