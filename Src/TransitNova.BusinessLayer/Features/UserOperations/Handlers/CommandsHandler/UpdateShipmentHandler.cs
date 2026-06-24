
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public class UpdateShipmentHandler(
        ILogger<UpdateShipmentHandler> logger,
        IShipmentQueryRepository shipmentQuery,
        IShipmentService shipmentService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
      : ICommandHandler<UpdateShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
        {
            //====== Fetch Shipment with Tracking for Update ======
            var shipment = await shipmentQuery.GetEntityAsync(request.ShipmentId, cancellationToken);
            if (shipment is null)
            {
                logger.LogWarning("Shipment with Id {ShipmentId} not found for update", request.ShipmentId);
                return BaseResult.NotFound(Errors.ShipmentNotFound($"Shipment with Id {request.ShipmentId} not found"));
            }

            shipmentService.UpdateShipmentDetails(shipment, request.Dto);
          
            //====== Save Changes and Handle Result ======
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            //====== Log Success and Return Result ======
            logger.LogInformation("Shipment {ShipmentId} updated successfully. New Cost: {Cost}", request.ShipmentId, shipment.ShipmentCost);
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
