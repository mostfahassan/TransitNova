
using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.CompleteShipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CompleteShipmentService;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.Services.CompleteShipmentService
{
    internal class CompleteShipmentService(
        ICarrierQueryRepository carrierQueryRepo,
        IShipmentQueryRepository shipmentQueryRepo,
         ISystemLogCommands systemLogCommands,
        ILogger<CompleteShipmentToWarehouseHandler> logger) : ICompleteShipmentService
    {
        public async Task<Shipment> CompleteShipmentAsync(Guid ShipmentId, Guid CarrierId, CancellationToken cancellationToken)
        {
            //===== 1- Retrieve Carrier  Attempts 
            var carrier = await carrierQueryRepo.GetCarrierAsync(c => c.Id == CarrierId, cancellationToken);
            if (carrier == null)
            {
                logger.LogError("carrier with {UserId} not found or not available.", CarrierId);
                throw new EntityNotFoundException($"carrier with {CarrierId} not found or not available.", "CARRIER_NOT_FOUND", nameof(Carrier));
            }
            //===== 2- Retrieve Shipment Attempts 
            var shipment = await shipmentQueryRepo.GetEntityAsync(ShipmentId, cancellationToken);
            if (shipment == null)
            {
                logger.LogError("Shipment with {shipmentId} not found", ShipmentId);
                throw new EntityNotFoundException($"Shipment with {ShipmentId} not found", "SHIPMENT_NOT_FOUND", nameof(Shipment));
            }

            //======= 3- Update Audits 
            carrier.CompleteShipment();
            shipment.Delivered(carrier.Id);
            if (shipment.Trip is { Status: TripStatus.Active } trip &&
                trip.Shipments.All(s => s.CurrentStatus == ShipmentStatuses.InWarehouse))
            {
                trip.Complete(carrier.Id);
            }

            var performedByName = (await carrierQueryRepo.GetCarrierNameAsync(CarrierId, cancellationToken))!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Completed,
                ActivityEntityType.Shipment,
                $"Pickup for shipment {ShipmentId} with tracking number {shipment.TrackingNumber} was completed at the warehouse.",
                CarrierId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);

            if (shipment.Trip is { Status: TripStatus.Completed } completedTrip)
            {
                log = SystemActivityLog.AddLog(
                    ActivityAction.Completed,
                    ActivityEntityType.Shipment,
                    $"Pickup trip {completedTrip.Id} was completed by carrier {CarrierId}.",
                    CarrierId,
                    performedByName);

                await systemLogCommands.LogAsync(log, cancellationToken);
            }
            return shipment;
        }

        public async Task<Shipment> CompleteShipmentToWarehouseAsync(Guid ShipmentId, Guid CarrierId, CancellationToken cancellationToken)
        {
            //===== 1- Retrieve Carrier  Attempts 
            var carrier = await carrierQueryRepo.GetCarrierAsync(c => c.Id == CarrierId, cancellationToken);
            if (carrier == null)
            {
                logger.LogError("carrier with {UserId} not found or not available.", CarrierId);
                throw new EntityNotFoundException($"carrier with {CarrierId} not found or not available.", "CARRIER_NOT_FOUND", nameof(Carrier));
            }
            //===== 2- Retrieve Shipment Attempts 
            var shipment = await shipmentQueryRepo.GetEntityAsync(ShipmentId, cancellationToken);
            if (shipment == null)
            {
                logger.LogError("Shipment with {shipmentId} not found", ShipmentId);
                throw new EntityNotFoundException($"Shipment with {ShipmentId} not found", "SHIPMENT_NOT_FOUND", nameof(Shipment));
            }

            carrier.CompleteShipment();
            shipment.DeliveredToWarehouse(carrier.Id);
            if (shipment.Trip is { Status: TripStatus.Active } trip &&
                trip.Shipments.All(s => s.CurrentStatus == ShipmentStatuses.Delivered))
            {
                trip.Complete(carrier.Id);
            }
            return shipment;
        }
    }
}
