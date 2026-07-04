
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Shipments
{
    public sealed class RejectShipmentHandler(IShipmentQueryRepository shipmentQueryRepo,
        ILogger<RejectShipmentHandler> logger,
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork)
        : ICommandHandler<RejectShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(RejectShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipmentToReject = await shipmentQueryRepo.GetShipmentInStatusAsync(request.ShipmentId, ShipmentStatuses.Pending, cancellationToken, true);
            if (shipmentToReject == null)
            {
                logger.LogWarning("Shipment with ID {ShipmentId} not found or not in pending status.", request.ShipmentId);
                return BaseResult.NotFound(Errors.InvalidShipmentState($"Shipment with ID {request.ShipmentId} not in pending status."));

            }
            var operationManagerId = await operationManagerRepository.GetUserIdAsync(request.OperationManagerId, cancellationToken);

            shipmentToReject.RejectShipment(operationManagerId, request.RejectionReason);
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Rejected,
                ActivityEntityType.Shipment,
                $"Shipment {request.ShipmentId} with tracking number {shipmentToReject.TrackingNumber} was rejected. Reason: {request.RejectionReason}",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            logger.LogInformation("Shipment with ID {ShipmentId} rejected by operation manager with ID {OperationManagerId}.", request.ShipmentId, request.OperationManagerId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Shipments.ByTrackingNumber(shipmentToReject.TrackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.OperationManagers.ShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}


