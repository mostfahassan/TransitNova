
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.CompleteShipmentService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;


namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.CompleteShipments
{
    public class CompleteShipmentToWarehouseHandler(
        ICompleteShipmentService completeShipmentService,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<CompleteShipmentToWarehouseHandler> logger)
        : ICommandHandler<CompleteShipmentToWarehouseCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CompleteShipmentToWarehouseCommand request, CancellationToken ct)
        {

            var shipment = await completeShipmentService.CompleteShipmentToWarehouseAsync(request.ShipmentId, request.CarrierId, ct);
            //======= 4- Save Changes
            await unitOfWork.SaveChangesAsync(ct);
           
            logger.LogInformation("Shipment {ShipmentId} completed successfully by Carrier {UserId} At {Time}", request.ShipmentId, request.CarrierId,DateTime.UtcNow);
            await cacheService.RemoveAsync(CacheKeys.CarrierShipmentDetails(request.CarrierId, request.ShipmentId));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierTrips(request.CarrierId));
            if (shipment.TripId.HasValue)
            {
                await cacheService.RemoveAsync(CacheKeys.CarrierTripDetails(request.CarrierId, shipment.TripId.Value));
            }
            await cacheService.RemoveAsync(CacheKeys.ShipmentByTrackingNumber(shipment.TrackingNumber));
            await cacheService.RemoveAsync(CacheKeys.OperationManagerDashboard());
            await cacheService.RemoveAsync(CacheKeys.OperationManagerShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}
