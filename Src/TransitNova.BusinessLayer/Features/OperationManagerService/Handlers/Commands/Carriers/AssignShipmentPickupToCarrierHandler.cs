
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Carriers;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Carriers
{
    public class AssignShipmentPickupToCarrierHandler(
        IShipmentAssignmentService shipmentAssignment,
        ILogger<AssignShipmentPickupToCarrierHandler> logger, 
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        IUnitOfWork unitOfWork)
        : ICommandHandler<AssignShipmentPickupToCarrierCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(AssignShipmentPickupToCarrierCommand request, CancellationToken cancellationToken)
        {
            // === assigning Shipment Logic 
            var assignedShipmentTrackingNumber = await shipmentAssignment.AssignPickupAsync(request.ShipmentId, request.OperationManagerId, request.CarrierId, cancellationToken);
            
            
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;

            var log = SystemActivityLog.AddLog(
                ActivityAction.Assigned,
                ActivityEntityType.Shipment,
                $"Shipment {request.ShipmentId} with tracking number {assignedShipmentTrackingNumber} was assigned to pickup carrier {request.CarrierId}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            // === Save Changes 
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Shipment with ID {ShipmentId} assigned to pickup carrier with ID {CarrierId} by operation manager with ID {OperationManagerId}.", request.ShipmentId, request.CarrierId, request.OperationManagerId);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Carriers.ShipmentDetails(request.CarrierId, request.ShipmentId),
                CacheKeys.Carriers.Dashboard(request.CarrierId),
                CacheKeys.Carriers.Trips(request.CarrierId),
                CacheKeys.Shipments.ByTrackingNumber(assignedShipmentTrackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.OperationManagers.ShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}


