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
    public sealed class UpdateTripHandler(
        ITripCommandRepository tripRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<UpdateTripHandler> logger)
        : ICommandHandler<UpdateTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetTripForCommandsAsync(request.TripId, cancellationToken);
            if (trip is null) return BaseResult.NotFound(Errors.TripNotFound($"Trip With Id => {request.TripId} Not Found"));

            var oldCarrierId = trip.CarrierId;

            trip.Update(
                request.Dto.CarrierId,
                request.Dto.WarehouseId,
                request.Dto.TripType,
                request.Dto.PlannedDate,
                request.Dto.StartTime,
                request.Dto.EndTime,
                request.Dto.TotalShipments ?? trip.TotalShipments);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await cacheService.RemoveAsync(CacheKeys.CarrierTrips(oldCarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierTripDetails(oldCarrierId, trip.Id));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(oldCarrierId));

            if (oldCarrierId != trip.CarrierId)
            {
                await cacheService.RemoveAsync(CacheKeys.CarrierTrips(trip.CarrierId));
                await cacheService.RemoveAsync(CacheKeys.CarrierTripDetails(trip.CarrierId, trip.Id));
                await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(trip.CarrierId));
            }

            await cacheService.RemoveAsync(CacheKeys.TripFilter(new FilterTripsDto()));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());

            logger.LogInformation("Trip updated successfully. TripId: {TripId}, OperationManagerId: {OperationManagerId}", trip.Id, request.OperationManagerId);
            return BaseResult.Success();
        }
    }
}
