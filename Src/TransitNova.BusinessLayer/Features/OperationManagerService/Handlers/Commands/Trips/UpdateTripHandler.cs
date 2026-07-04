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
    public sealed class UpdateTripHandler(
        ITripCommandRepository tripRepository,
        IUnitOfWork unitOfWork,
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
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Carriers.Trips(oldCarrierId),
                CacheKeys.Carriers.TripDetails(oldCarrierId, trip.Id),
                CacheKeys.Carriers.Dashboard(oldCarrierId),
                oldCarrierId != trip.CarrierId ? CacheKeys.Carriers.Trips(trip.CarrierId) : string.Empty,
                oldCarrierId != trip.CarrierId ? CacheKeys.Carriers.TripDetails(trip.CarrierId, trip.Id) : string.Empty,
                oldCarrierId != trip.CarrierId ? CacheKeys.Carriers.Dashboard(trip.CarrierId) : string.Empty,
                CacheKeys.Trips.Details(trip.Id),
                CacheKeys.Trips.Filter(new FilterTripsDto()),
                CacheKeys.OperationManagers.OperationManagersDashboard);

            logger.LogInformation("Trip updated successfully. TripId: {TripId}, OperationManagerId: {OperationManagerId}", trip.Id, request.OperationManagerId);
            return BaseResult.Success();
        }
    }
}


