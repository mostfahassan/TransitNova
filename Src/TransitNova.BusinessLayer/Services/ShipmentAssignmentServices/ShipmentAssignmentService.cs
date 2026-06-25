
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Services.ShipmentAssignmentServices
{
    internal class ShipmentAssignmentService(IShipmentQueryRepository shipment,
        ICarrierQueryRepository carrier,
        IOperationManagerQueryRepository operationManagerRepository,
        ILogger<AssignShipmentPickupToCarrierHandler> logger) : IShipmentAssignmentService
    {
        public async Task<string> AssignDeliveryAsync(Guid ShipmentId, Guid OperationManagerId, Guid CarrierId, CancellationToken cancellationToken)
        {
            //==== Check Existence Of The Shipment
            var shipmentToDeliver = await shipment.GetShipmentInStatusAsync(ShipmentId, ShipmentStatuses.InWarehouse, cancellationToken, true);
            if (shipmentToDeliver == null)
            {
                logger.LogError("Shipment with {shipmentId} not found", ShipmentId);
                throw new EntityNotFoundException($"Shipment with {ShipmentId} not found", "SHIPMENT_NOT_FOUND", nameof(Shipment));
            }

            //===== Check Existence Of The Carrier And Assign Shipment To It
            var assignedCarrier = await carrier.GetCarrierAsync(c => c.Id == CarrierId, cancellationToken);
            if (assignedCarrier == null)
            {
                logger.LogError("carrier with {UserId} not found or not available.", CarrierId);
                throw new EntityNotFoundException($"carrier with {CarrierId} not found or not available.", "CARRIER_NOT_FOUND", nameof(Carrier));
            }

            //=== Get Operation Manager Id For Audit
            var operationManagerId = await operationManagerRepository.GetUserIdAsync(OperationManagerId, cancellationToken);

            //==== Assign Shipment To The Carrier and Update Shipment Status
            assignedCarrier.AssignToDeliver(operationManagerId);
            shipmentToDeliver.AssignToCarrier(ShipmentStatuses.AssignedToDeliveryCarrier, assignedCarrier.Id, operationManagerId);

            return shipmentToDeliver.TrackingNumber;
        }

        public async Task<string> AssignPickupAsync(Guid ShipmentId, Guid OperationManagerId, Guid CarrierId, CancellationToken cancellationToken)
        {
            //==== Check Existence Of The Shipment =========
            var shipmentToPickUp = await shipment.GetShipmentInStatusAsync(ShipmentId, ShipmentStatuses.AssignedToPickUpCarrier, cancellationToken, true);
            if (shipmentToPickUp == null)
            {
                logger.LogError("Shipment with {shipmentId} not found", ShipmentId);
                throw new EntityNotFoundException($"Shipment with {ShipmentId} not found", "SHIPMENT_NOT_FOUND", nameof(Shipment));
            }

            //===== Check Existence Of The Carrier And Assign Shipment To It  =========
            var assignedCarrier = await carrier.GetCarrierAsync(c => c.Id == CarrierId, cancellationToken);
            if (assignedCarrier == null)
            {
                logger.LogError("carrier with {UserId} not found or not available.", CarrierId);
                throw new EntityNotFoundException($"carrier with {CarrierId} not found or not available.", "CARRIER_NOT_FOUND", nameof(Carrier));
            }

            //=== Get Operation Manager Id For Audit
            var operationManagerId = await operationManagerRepository.GetUserIdAsync(OperationManagerId, cancellationToken);

            //==== Assign Shipment To The Carrier and Update Shipment Status
            assignedCarrier.AssignToPickup(operationManagerId);
            shipmentToPickUp.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, assignedCarrier.Id, operationManagerId);

            return shipmentToPickUp.TrackingNumber;
        }
    }
}
