using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.TripService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Trips
{
    public class StartPickupTripHandler(
        ITripServices tripServices,
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ILogger<StartPickupTripHandler> logger) : ICommandHandler<StartPickupTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(StartPickupTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await tripServices.StartPickupTripAsync(request.OperationManagerId,request.CarrierId, cancellationToken);
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Trip,
                $"Pickup trip {trip.Id} was created for carrier {trip.CarrierId} with {trip.TotalShipments} shipments.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);

            log = SystemActivityLog.AddLog(
                ActivityAction.Started,
                ActivityEntityType.Trip,
                $"Pickup trip {trip.Id} was started for carrier {trip.CarrierId}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);

            log = SystemActivityLog.AddLog(
                ActivityAction.Started,
                ActivityEntityType.Shipment,
                $"Pickup started for {trip.TotalShipments} shipments on trip {trip.Id}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Trip started successfully by operation Manager {UserId}. TripId: {TripId}, UserId: {UserId}, StartedAt: {StartedAt}", request.OperationManagerId, trip.Id, trip.CarrierId, DateTime.UtcNow);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Carriers.Trips(request.CarrierId),
                CacheKeys.Carriers.TripDetails(request.CarrierId, trip.Id),
                CacheKeys.Carriers.Dashboard(request.CarrierId),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.Trips.Details(trip.Id));
            return BaseResult.Success();
        }
    }
}



