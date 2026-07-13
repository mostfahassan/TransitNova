using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Services.CompleteShipmentService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.CompleteShipments
{
    public class PickedUpShipmentCommandHandler(
        ICompleteShipmentService completeShipmentService,
        IUnitOfWork unitOfWork,
        ILogger<PickedUpShipmentCommandHandler> logger)
        : ICommandHandler<PickedUpShipmentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(PickedUpShipmentCommand request, CancellationToken ct)
        {
            var shipment = await completeShipmentService.PickedUpShipmentAsync(request.ShipmentId, request.CarrierId, ct);

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Shipment {ReferecneId} picked up successfully by Carrier {CarrierId} At {Time}", request.ShipmentId, request.CarrierId, DateTime.UtcNow);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Carriers.ShipmentDetails(request.CarrierId, request.ShipmentId),
                CacheKeys.Carriers.Dashboard(request.CarrierId),
                CacheKeys.Carriers.Trips(request.CarrierId),
                shipment.TripId.HasValue ? CacheKeys.Carriers.TripDetails(request.CarrierId, shipment.TripId.Value) : string.Empty,
                CacheKeys.Shipments.ByTrackingNumber(shipment.TrackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard,
                CacheKeys.OperationManagers.ShipmentHistories(request.ShipmentId));
            return BaseResult.Success();
        }
    }
}
