
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
    public class DeleteShipmentHandler(
     IShipmentQueryRepository shipmentRepo,
     IUnitOfWork unitOfWork,
     ILogger<DeleteShipmentHandler> logger)
     : ICommandHandler<DeleteShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
           
            logger.LogDebug("Attempting soft-delete for Shipment {ReferecneId}", request.ShipmentId);
            //====== Soft Delete Operation ======
            var shipment = await shipmentRepo.GetShipmentForCommandsAsync(request.ShipmentId, cancellationToken);
            if (shipment == null)
            {
                logger.LogWarning("Shipment with Id: {ReferecneId} not found for soft-delete", request.ShipmentId);
                return BaseResult.Failure(Errors.ShipmentNotFound($"Shipment With Id =>  {request.ShipmentId} not found"));
            }
            //====== Perform soft delete by marking the shipment as deleted ======
            shipment.DeleteShipment();
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Shipment {ReferecneId} soft-deleted successfully", request.ShipmentId);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Users.Dashboard(request.AppUserId),
                CacheKeys.Users.Profile(request.AppUserId),
                CacheKeys.Users.AdminDetails(request.AppUserId),
                CacheKeys.Users.Shipment(request.AppUserId, request.ShipmentId),
                CacheKeys.Shipments.ByTrackingNumber(shipment.TrackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.OperationManagers.ShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}


