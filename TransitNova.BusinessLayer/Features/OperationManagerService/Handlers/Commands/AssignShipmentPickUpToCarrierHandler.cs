
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands
{
    public class AssignShipmentPickUpToCarrierHandler(
        IShipmentAssignmentService shipmentAssignment,
        ILogger<AssignShipmentPickUpToCarrierHandler> logger, 
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<AssignShipmentPickUpToCarrierCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(AssignShipmentPickUpToCarrierCommand request, CancellationToken cancellationToken)
        {
            // === assigning Shipment Logic 
            var assignedShipmentTrackingNumber = await shipmentAssignment.AssignPickup(request.ShipmentId, request.OperationManagerId, request.CarrierId, cancellationToken);
            
            
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;

            var log = SystemActivityLog.AddLog(
                ActivityAction.Assigned,
                ActivityEntityType.Shipment,
                $"Shipment {request.ShipmentId} with tracking number {assignedShipmentTrackingNumber} was assigned to pickup carrier {request.CarrierId}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.Log(log, cancellationToken);
            // === Save Changes 
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Shipment with ID {ShipmentId} assigned to delivery carrier with ID {UserId} by operation manager with ID {OperationManagerId}.", request.ShipmentId, request.CarrierId, request.OperationManagerId); ;

            // === Invalidate Cache
            await cacheService.RemoveAsync(CacheKeys.CarrierShipmentDetails(request.CarrierId, request.ShipmentId));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierTrips(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.ShipmentByTrackingNumber(assignedShipmentTrackingNumber));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            await cacheService.RemoveAsync(CacheKeys.OperationManagerShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}
