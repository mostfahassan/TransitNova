using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Trips
{
    public sealed class CompleteCarrierTripCommandHandler(
        ITripCommandRepository tripCommandRepository,
        ICarrierQueryRepository carrierQueryRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ILogger<CompleteCarrierTripCommandHandler> logger)
        : ICommandHandler<CompleteCarrierTripCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CompleteCarrierTripCommand request, CancellationToken ct)
        {
            var trip = await tripCommandRepository.GetTripForCommandsAsync(request.TripId, ct);
            if (trip is null)
                throw new EntityNotFoundException($"Trip with {request.TripId} was not found.", "TRIP_NOT_FOUND", nameof(Trip));

            trip.Complete(request.CarrierId);

            var performedByName = await carrierQueryRepository.GetCarrierNameAsync(request.CarrierId, ct) ?? request.CarrierId.ToString();
            var log = SystemActivityLog.AddLog(
                ActivityAction.Completed,
                ActivityEntityType.Trip,
                $"Carrier {request.CarrierId} completed {trip.TripType} trip {trip.Id} with {trip.TotalShipments} shipments.",
                request.CarrierId,
                performedByName);

            await systemLogCommands.LogAsync(log, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Carrier {CarrierId} completed trip {TripId} at {Time}.", request.CarrierId, request.TripId, DateTime.UtcNow);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Carriers.Trips(request.CarrierId),
                CacheKeys.Carriers.TripDetails(request.CarrierId, request.TripId),
                CacheKeys.Carriers.Dashboard(request.CarrierId),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.Trips.Details(request.TripId));

            return BaseResult.Success();
        }
    }
}