using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.TripService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands
{
    public class StartPickupTripHandler(
        ITripServices tripServices,
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<StartPickupTripHandler> logger) : ICommandHandler<StartPickUpTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(StartPickUpTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await tripServices.StartPickUpTripAsync(request.OperationManagerId,request.CarrierId, cancellationToken);
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Trip,
                $"Pickup trip {trip.Id} was created for carrier {trip.CarrierId} with {trip.TotalShipments} shipments.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.Log(log, cancellationToken);

            log = SystemActivityLog.AddLog(
                ActivityAction.Started,
                ActivityEntityType.Trip,
                $"Pickup trip {trip.Id} was started for carrier {trip.CarrierId}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.Log(log, cancellationToken);

            log = SystemActivityLog.AddLog(
                ActivityAction.Started,
                ActivityEntityType.Shipment,
                $"Pickup started for {trip.TotalShipments} shipments on trip {trip.Id}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.Log(log, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Trip started successfully by operation Manager {UserId}. TripId: {TripId}, UserId: {UserId}, StartedAt: {StartedAt}", request.OperationManagerId, trip.Id, trip.CarrierId, DateTime.UtcNow);
            await cacheService.RemoveAsync(CacheKeys.CarrierTrips(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierTripDetails(request.CarrierId, trip.Id));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            return BaseResult.Success();

        }

    }
}

