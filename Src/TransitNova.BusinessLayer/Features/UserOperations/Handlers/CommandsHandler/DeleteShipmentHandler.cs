
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public class DeleteShipmentHandler(
     IShipmentQueryRepository shipmentRepo,
     IUnitOfWork unitOfWork,
     ICacheService cacheService,
     ILogger<DeleteShipmentHandler> logger)
     : ICommandHandler<DeleteShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
           
            logger.LogDebug("Attempting soft-delete for Shipment {ShipmentId}", request.ShipmentId);
            //====== Soft Delete Operation ======
            var shipment = await shipmentRepo.GetShipmentForCommandsAsync(request.ShipmentId, cancellationToken);
            if (shipment == null)
            {
                logger.LogWarning("Shipment with Id: {ShipmentId} not found for soft-delete", request.ShipmentId);
                return BaseResult.Failure(Errors.ShipmentNotFound($"Shipment With Id =>  {request.ShipmentId} not found"));
            }
            //====== Perform soft delete by marking the shipment as deleted ======
            shipment.DeleteShipment();
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Shipment {ShipmentId} soft-deleted successfully", request.ShipmentId);
            await cacheService.RemoveAsync(CacheKeys.UserDashboard(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.UserProfile(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.AdminUserDetails(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.UserShipment(request.AppUserId, request.ShipmentId));
            await cacheService.RemoveAsync(CacheKeys.ShipmentByTrackingNumber(shipment.TrackingNumber));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            await cacheService.RemoveAsync(CacheKeys.OperationManagerShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}
