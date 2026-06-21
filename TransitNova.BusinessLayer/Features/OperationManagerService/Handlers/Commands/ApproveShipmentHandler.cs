
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands
{
    public class ApproveShipmentHandler(
        IShipmentQueryRepository shipment,
        ILogger<ApproveShipmentHandler> logger,
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<ApproveShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(ApproveShipmentCommand request, CancellationToken cancellationToken)
        {
            // Get The Request Shipment ======
            var pendedingShipment = await shipment.GetShipmentInStatusAsync(request.ShipmentId, ShipmentStatuses.Pending, cancellationToken, false);
            //==== Check Existence Of The Shipment
            if (pendedingShipment == null)
            {
                return BaseResult.NotFound(Errors.InvalidShipmentState($"Shipment with ID {request.ShipmentId} is not pending"));
            }

            //=== Get Operation Manager Id For Audit ========
            var operationManagerId = await operationManagerRepository.GetUserIdAsync(request.OperationManagerId, cancellationToken);

            //====== Approve Shhipment ========
            pendedingShipment.ApproveShipment(operationManagerId);

            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Approved,
                ActivityEntityType.Shipment,
                $"Shipment {request.ShipmentId} with tracking number {pendedingShipment.TrackingNumber} was approved.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.Log(log, cancellationToken);

            //====== Saving Chhanges ===========
             await unitOfWork.SaveChangesAsync(cancellationToken);
         
            logger.LogInformation("Shipment {ShipmentId} approved by {ManagerId}.", request.ShipmentId, request.OperationManagerId);
            await cacheService.RemoveAsync(CacheKeys.ShipmentByTrackingNumber(pendedingShipment.TrackingNumber));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            await cacheService.RemoveAsync(CacheKeys.OperationManagerShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}
