
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment
{
    public class CancelShipmentHandler(
        IShipmentQueryRepository shipmentRepo,
        IUserAuthQueryService userQuery,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<CancelShipmentHandler> logger)
      : ICommandHandler<CancelShipmentCommand, BaseResult>
    {

        public async Task<BaseResult> Handle(CancelShipmentCommand request, CancellationToken cancellationToken)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Attempting cancellation for ShipmentId: {ShipmentId}", request.ShipmentId);
            }
            //====== Cancellation Attempt ======
            var cancelledShipment = await shipmentRepo.GetShipmentForCommandsAsync(request.ShipmentId, cancellationToken);
            if (cancelledShipment == null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Shipment with Id: {ShipmentId} not found for cancellation", request.ShipmentId);
                }

                return BaseResult.NotFound(Errors.ShipmentNotFound($"Shipment With Id =>  {request.ShipmentId} not found"));
            }
            cancelledShipment.CancelShipment();
            var performedByName = (await userQuery.FindByIdAsync(request.AppUserId, cancellationToken))!.FullName!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Cancelled,
                ActivityEntityType.Shipment,
                $"Shipment {request.ShipmentId} with tracking number {cancelledShipment.TrackingNumber} was cancelled.",
                request.AppUserId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (result < 0) throw new DomainOperationException($"Failed to Cancle Shipment {request.ShipmentId}", "SHIPMENT_CANCELLATION_FAILED");

            //====== Cancellation Successful ======
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Shipment {ShipmentId} cancelled successfully", request.ShipmentId);
            }
            await cacheService.RemoveAsync(CacheKeys.UserDashboard(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.UserProfile(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.AdminUserDetails(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.UserShipment(request.AppUserId, request.ShipmentId));
            await cacheService.RemoveAsync(CacheKeys.ShipmentByTrackingNumber(cancelledShipment.TrackingNumber));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            await cacheService.RemoveAsync(CacheKeys.OperationManagerShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }

    }
}

