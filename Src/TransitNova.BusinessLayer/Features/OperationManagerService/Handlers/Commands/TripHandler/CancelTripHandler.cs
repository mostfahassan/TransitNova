using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.TripHandler
{
    public sealed class CancelTripHandler(
        ITripCommandRepository tripRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<CancelTripHandler> logger)
        : ICommandHandler<CancelTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripForCommandsAsync(request.TripId, cancellationToken);
            if (trip is null) return BaseResult.NotFound(Errors.TripNotFound($"Trip With Id => {request.TripId} Not Found"));

            trip.Cancel(request.OperationManagerId);
       
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await cacheService.RemoveAsync(CacheKeys.CarrierTrips(trip.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierTripDetails(trip.CarrierId, trip.Id));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(trip.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.TripFilter(new FilterTripsDto()));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());

            logger.LogInformation("Trip cancelled successfully. TripId: {TripId}, OperationManagerId: {OperationManagerId}", trip.Id, request.OperationManagerId);
            return BaseResult.Success();
        }
    }
}
