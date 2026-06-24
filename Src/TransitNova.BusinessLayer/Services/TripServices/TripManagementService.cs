
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.TripService;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.Services.TripServices
{
    internal class TripManagementService(
        IOperationManagerQueryRepository operationManagerRepository,
        ILogger<TripManagementService> logger,
        ITripCommandRepository tripRepository,
        IShipmentQueryRepository shipment,
        ICarrierQueryRepository carrierRep) : ITripServices
    {
        public async Task<Trip> StartDeliveryTripAsync(Guid operationManagerId, Guid carrierId, CancellationToken cancellationToken)
        {  
            
            // ========= 1 - Check if the carrier is available and can be assigned to a delivery trip
            var carrier = await carrierRep.GetCarrierForTripAsync(c => c.Id == carrierId, cancellationToken);
            if (carrier == null )
            {
                logger.LogError("carrier with {UserId} not found or not available.",carrierId);
                throw new EntityNotFoundException($"carrier with {carrierId} not found or not available.", "CARRIER_NOT_FOUND", nameof(Carrier));
            }
            if (carrier.Status != CarrierStatus.AssignedToDeliveryShipment)
            {
                logger.LogError("carrier with {UserId} not found or not available.",carrierId);
                throw new InvalidCarrierStatusException();
            }

            //===== 2- Get the list of shipments assigned to the carrier for delivery and ensure they are in the correct status (InWarehouse)
            var shipmentList = (await shipment.GetShipmentsAssignedToCarrierAsync(ShipmentStatuses.AssignedToDeliveryCarrier, carrierId, cancellationToken)).ToList();

            // ===== Validate that there are shipments assigned to the carrier for delivery
            if (shipmentList.Count is 0)
            {
                logger.LogWarning("No shipments assigned to carrier {UserId} for Pickup.", carrierId);
                throw new InvalidOperationException("An error occurred while starting Pickup trip,No shipments found.");
            }

            // === Get Receiver Info Ready for the trip to be used in the notification service to notify the receivers about the trip details
            var receiverContactInfo = new Dictionary<Guid, ContactInfo>();
            foreach (var shipment in shipmentList)
            {
                if (shipment.Receiver is null || shipment.Sender is null) throw new ArgumentException("Shipment Receiver Or Sender Can't Be Null");

                var receiverContact = new ContactInfo(shipment.Receiver.PhoneNumber, shipment.Receiver.Address);

                if (!receiverContactInfo.ContainsKey(shipment.Receiver.Id)) continue;
                receiverContactInfo.Add(shipment.Receiver.Id, receiverContact);
            }

            // === Get the warehouse information to be used in the trip details
            var warehouseId = carrier.HomeWarehouseId ?? Guid.Empty;
      
            //=== Get Operation Manager Id For Audit 
            var operationManagerProfileId = await operationManagerRepository.GetUserIdAsync(operationManagerId, cancellationToken);

            // === Plan the trip, update the status of the shipments to OutForPickup, and assign the carrier to the trip
            var trip = Trip.Plan(carrier.Id, warehouseId, TripType.Delivery, shipmentList);
            trip.StartTrip(operationManagerProfileId,TripType.Delivery);
            foreach (var s in shipmentList)
                s.AssignedAsDeliveryTrip(trip.Id, carrier.Id);

            //=== Update the carrier's status to AssignedToTrip and assign it to the trip with the number of shipments it will be picking up
            carrier.AssignToTrip(operationManagerProfileId,trip.Id);

            // === Save the changes to the database and commit the transaction
            await tripRepository.StartNewTripAsync(trip, cancellationToken);

            return trip;
        }

        public async Task<Trip> StartPickupTripAsync(Guid operationManagerId, Guid carrierId, CancellationToken cancellationToken)
        { 
            
            // ========= 1 - Check if the carrier is available and can be assigned to a delivery trip
            var carrier = await carrierRep.GetCarrierForTripAsync(c => c.Id == carrierId, cancellationToken);
            if (carrier == null)
            {
                logger.LogError("carrier with {UserId} not found or not available.", carrierId);
                throw new EntityNotFoundException($"carrier with {carrierId} not found or not available.", "CARRIER_NOT_FOUND", nameof(Carrier));
            }
            if (carrier.Status != CarrierStatus.AssignedToPickUpShipment)
            {
                logger.LogError("carrier with {UserId} not found or not available.", carrierId);
                throw new InvalidCarrierStatusException();
            }

            //===== 2- Get the list of shipments assigned to the carrier for delivery and ensure they are in the correct status (InWarehouse)
            var shipmentList = (await shipment.GetShipmentsAssignedToCarrierAsync(ShipmentStatuses.AssignedToPickUpCarrier, carrierId, cancellationToken)).ToList();

            // ===== Validate that there are shipments assigned to the carrier for delivery
            if (shipmentList.Count is 0)
            {
                logger.LogWarning("No shipments assigned to carrier {UserId} for Pickup.", carrierId);
                throw new InvalidOperationException("An error occurred while starting Pickup trip,No shipments found.");
            }
            // ===== 3- Get Sender Info Ready for the trip to be used in the notification service to notify the receivers about the trip details
            var senderContactInfo = new Dictionary<Guid, ContactInfo>();
            foreach (var shipment in shipmentList)
            {
                if (shipment.Receiver is null || shipment.Sender is null) throw new ArgumentException("Shipment Receiver Or Sender Can't Be Null");
                var senderContact = new ContactInfo(shipment.Sender.PhoneNumber, shipment.Sender.Address);
                // Handle If Sender Has Many Sent Shipments
                if (!senderContactInfo.ContainsKey(shipment.Sender.Id)) continue;
                senderContactInfo.Add(shipment.Sender.Id, senderContact);
            }

            // ===== 4- Get the warehouse information to be used in the trip details
            var warehouseId = carrier.HomeWarehouseId ?? Guid.Empty;

            //=== Get Operation Manager Id For Audit ========
            var operationManagerProfileId = await operationManagerRepository.GetUserIdAsync(operationManagerId, cancellationToken);

            // ===== 5- Plan the trip, update the status of the shipments to OutForPickup, and assign the carrier to the trip
            var trip = Trip.Plan(carrier.Id, warehouseId, TripType.Pickup, shipmentList);
            trip.StartTrip(operationManagerProfileId, TripType.Pickup);
            foreach (var s in shipmentList)
                s.AssignedAsPickupTrip(trip.Id,carrier.Id);

            //===== 6- Update the carrier's status to AssignedToTrip and assign it to the trip with the number of shipments it will be picking up
            carrier.AssignToTrip(operationManagerProfileId, trip.Id);

            // ===== 7- Save the changes to the database and commit the transaction
            await tripRepository.StartNewTripAsync(trip, cancellationToken);
            return trip;
        }
    }
}
