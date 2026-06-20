using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands
{
    public class AssignShipmentDeliveryToCarrierHandler(
        IShipmentAssignmentService shipmentAssignment,
        ILogger<AssignShipmentDeliveryToCarrierHandler> logger,
        IOperationManagerQueryRepository operationManagerRepository,
        ISystemLogCommands systemLogCommands,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
        : ICommandHandler<AssignShipmentDeliveryToCarrierCommand, BaseResult>, ITransactional
    {
        public async Task<BaseResult> Handle(AssignShipmentDeliveryToCarrierCommand request, CancellationToken cancellationToken)
        {
           // === assigning Shipment Logic 
           var assignedShipmentTrackingNumber = await shipmentAssignment.AssignDelivery(request.ShipmentId,request.OperationManagerId, request.CarrierId, cancellationToken);
            var performedByName = (await operationManagerRepository.GetOperationManagerNameAsync(
                request.OperationManagerId,
                cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Assigned,
                ActivityEntityType.Shipment,
                $"Shipment {request.ShipmentId} with tracking number {assignedShipmentTrackingNumber} was assigned to delivery carrier {request.CarrierId}.",
                request.OperationManagerId,
                performedByName);

            await systemLogCommands.Log(log, cancellationToken);
            //==== Save Changes 
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
