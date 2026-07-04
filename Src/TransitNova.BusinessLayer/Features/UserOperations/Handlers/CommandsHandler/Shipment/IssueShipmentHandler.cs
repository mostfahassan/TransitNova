
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment
{
    public class IssueShipmentHandler(
         IShipmentQueryRepository shipmentRepo,
         IUnitOfWork unitOfWork,
         ILogger<IssueShipmentHandler> logger)
       : ICommandHandler<IssueShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(IssueShipmentCommand request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Attempting issue for ShipmentId: {ShipmentId}", request.ShipmentId);
            //====== Issue Shipment ======
            var issued = await shipmentRepo.GetShipmentForCommandsAsync(request.ShipmentId, cancellationToken);
            if (issued is null)
            {
                logger.LogWarning("Issue executed but no changes saved for {ShipmentId}", request.ShipmentId);
                
                return BaseResult.Failure(Errors.FailedOperation($"Issue Failed For Shipment With Id =>  {request.ShipmentId} not found"));
            }

            issued.IssueShipment(request.IssueMessage);
         
            await unitOfWork.SaveChangesAsync(cancellationToken);
           
            //====== Handle Failure to Issue ======
            logger.LogInformation("Shipment {ShipmentId} issued successfully", request.ShipmentId);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Users.Dashboard(request.AppUserId),
                CacheKeys.Users.Profile(request.AppUserId),
                CacheKeys.Users.AdminDetails(request.AppUserId),
                CacheKeys.Users.Shipment(request.AppUserId, request.ShipmentId),
                CacheKeys.Shipments.ByTrackingNumber(issued.TrackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.OperationManagers.ShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}


