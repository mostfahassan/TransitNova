using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.TripService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.TripHandler
{
    public class StartDeliveryTripHandler(
        ITripServices tripServices,
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<StartDeliveryTripHandler> logger) : ICommandHandler<StartDeliveryTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(StartDeliveryTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await tripServices.StartDeliveryTripAsync(request.OperationManagerId,request.CarrierId,cancellationToken);
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Trip,
                $"Delivery trip {trip.Id} was created for carrier {trip.CarrierId} with {trip.TotalShipments} shipments.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);

            log = SystemActivityLog.AddLog(
                ActivityAction.Started,
                ActivityEntityType.Trip,
                $"Delivery trip {trip.Id} was started for carrier {trip.CarrierId}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);

            log = SystemActivityLog.AddLog(
                ActivityAction.Started,
                ActivityEntityType.Shipment,
                $"Delivery started for {trip.TotalShipments} shipments on trip {trip.Id}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
         
            logger.LogInformation("Trip started successfully by OperationManager {OperationManagerId}. TripId: {TripId}, CarrierId: {CarrierId}, StartedAt: {StartedAt}", request.OperationManagerId, trip.Id, trip.CarrierId, DateTime.UtcNow);
            await cacheService.RemoveAsync(CacheKeys.CarrierTrips(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierTripDetails(request.CarrierId, trip.Id));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            return BaseResult.Success();
        }
    }
}

