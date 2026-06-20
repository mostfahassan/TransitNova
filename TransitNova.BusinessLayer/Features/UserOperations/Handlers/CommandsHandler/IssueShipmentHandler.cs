
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
 
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public class IssueShipmentHandler(
         IShipmentQueryRepository shipmentRepo,
         IUnitOfWork unitOfWork,
         ICacheService cacheService,
         ILogger<IssueShipmentHandler> logger)
       : ICommandHandler<IssueShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(IssueShipmentCommand request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Attempting issue for ShipmentId: {ShipmentId}", request.ShipmentId);
            //====== Issue Shipment ======
            var issued = await shipmentRepo.GetShipmentForCommands(request.ShipmentId, cancellationToken);
            if (issued is null)
            {
                logger.LogWarning("Issue executed but no changes saved for {ShipmentId}", request.ShipmentId);
                
                return BaseResult.Failure(Errors.FailedOperation($"Issue Failed For Shipment With Id =>  {request.ShipmentId} not found"));
            }

            issued.IssueShipment(request.IssueMessage);
         
            await unitOfWork.SaveChangesAsync(cancellationToken);
           
            //====== Handle Failure to Issue ======
            logger.LogInformation("Shipment {ShipmentId} issued successfully", request.ShipmentId);
            await cacheService.RemoveAsync(CacheKeys.UserDashboard(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.UserProfile(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.AdminUserDetails(request.AppUserId));
            await cacheService.RemoveAsync(CacheKeys.UserShipment(request.AppUserId, request.ShipmentId));
            await cacheService.RemoveAsync(CacheKeys.ShipmentByTrackingNumber(issued.TrackingNumber));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            await cacheService.RemoveAsync(CacheKeys.OperationManagerShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}
