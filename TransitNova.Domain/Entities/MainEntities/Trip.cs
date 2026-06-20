
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Trip : BaseEntity<Guid>
    {
        private readonly List<Shipment> _shipments = new();
        public Guid CarrierId { get; private set; }
        public Carrier Carrier { get; set; } = null!;
        public Guid WarehouseId { get; private set; }
        public byte[] RowVersion { get; } = default!; 
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual IReadOnlyCollection<Shipment> Shipments => _shipments;
        public TripType TripType { get; private set; }
        public TripStatus Status { get; private set; }
        public DateTime PlannedDate { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int TotalShipments { get; private set; }
        
        // static factory method for creating a new trip
        private Trip()
        {
        }
        private Trip(Guid carrierId, Guid warehouseId, TripType tripType, List<Shipment> shipments)
        {
            ValidatePlan(carrierId, warehouseId, tripType, shipments);

            Id = Guid.CreateVersion7();
            CarrierId = carrierId;
            WarehouseId = warehouseId;
            TripType = tripType;
            TotalShipments = shipments.Count;
            Status = TripStatus.Planned;
            PlannedDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            _shipments.AddRange(shipments);
        }

        static void ValidatePlan(Guid carrierId, Guid warehouseId, TripType tripType, List<Shipment> shipments)
        {
            if (carrierId == Guid.Empty)
                throw new DomainArgumentException(nameof(carrierId), "Carrier is required.", "ARG_CARRIER_REQUIRED", "Trip");

            if (warehouseId == Guid.Empty)
                throw new DomainArgumentException(nameof(warehouseId), "Warehouse is required.", "ARG_WAREHOUSE_REQUIRED", "Trip");

            if (shipments is null || shipments.Count == 0)
                throw new TripPlanningException("Trip must contain at least one shipment.");

            if (shipments.Any(s => s is null))
                throw new DomainArgumentException(nameof(shipments), "Trip shipments cannot contain null values.", "ARG_SHIPMENT_NULL", "Trip");

            if (shipments.Select(s => s.Id).Distinct().Count() != shipments.Count)
                throw new DuplicateShipmentInTripException("Duplicate shipments cannot be added to the same trip.", "DUPLICATE_SHIPMENT_IN_TRIP", "Trip");

            if (shipments.Any(s => s.TripId.HasValue))
                throw new TripPlanningException("One or more shipments are already assigned to a trip.");

            if (tripType == TripType.Pickup && shipments.Any(s => s.CurrentStatus != Enums.Shipment.ShipmentStatuses.AssignedToPickUpCarrier))
                throw new TripPlanningException("Pickup trips can only include shipments assigned to a pickup carrier.");

            if (tripType == TripType.Delivery && shipments.Any(s => s.CurrentStatus != Enums.Shipment.ShipmentStatuses.InWarehouse))
                throw new TripPlanningException("Delivery trips can only include shipments that are in warehouse.");
        }

        private void EnsurePlanned()
        {
            if (Status != TripStatus.Planned)
                throw new DomainOperationException($"Trip must be planned before it can be started. Current status: {Status}.", "INVALID_TRIP_STATUS", "Trip", Id);
        }

        private void EnsureActive()
        {
            if (Status != TripStatus.Active)
                throw new DomainOperationException($"Trip must be active before it can be completed. Current status: {Status}.", "INVALID_TRIP_STATUS", "Trip", Id);
        }

        private void EnsureType(TripType expectedType)
        {
            if (TripType != expectedType)
                throw new TripPlanningException($"Trip must be a {expectedType} trip. Current type: {TripType}.", Id);
        }

        public void Update(Guid carrierId, Guid warehouseId, TripType tripType,
            TripStatus status, DateTime plannedDate,
            DateTime? startTime, DateTime? endTime, int totalShipments)

        {
            CarrierId = carrierId;
            WarehouseId = warehouseId;
            TripType = tripType;
            Status = status;
            PlannedDate = plannedDate;
            StartTime = startTime;
            EndTime = endTime;
            TotalShipments = totalShipments;
            UpdatedAt = DateTime.UtcNow;
        }
        public static Trip Plan(Guid carrierId, Guid warehouseId, TripType tripType, List<Shipment> shipments)
            => new(carrierId, warehouseId, tripType, shipments);

        public void StartPickup(Guid operationManagerId)
        {
            EnsurePlanned();
            EnsureType(TripType.Pickup);

            Status = TripStatus.Active;
            StartTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = operationManagerId.ToString();
        }

        public void StartDelivery(Guid operationManagerId)
        {
            EnsurePlanned();
            EnsureType(TripType.Delivery);

            Status = TripStatus.Active;
            StartTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = operationManagerId.ToString();
        }

        public void Complete(Guid carrierId)
        {
            EnsureActive();

            if (CarrierId != carrierId)
                throw new DomainOperationException("Only the assigned carrier can complete this trip.", "TRIP_CARRIER_MISMATCH", "Trip", Id);

            Status = TripStatus.Completed;
            EndTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = carrierId.ToString();
        }

        public void Cancel(Guid operationManagerId)
        {
            if (Status == TripStatus.Completed)
                throw new DomainOperationException("Completed trips cannot be cancelled.", "COMPLETED_TRIP_CANNOT_BE_CANCELLED", "Trip", Id);

            Status = TripStatus.Cancelled;
            EndTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = operationManagerId.ToString();
        }


        public void AddShipment(Shipment shipment, Guid operationManagerId)
        {
            if (shipment is null)
                throw new DomainArgumentException(nameof(shipment), "Shipment cannot be null.", "ARG_SHIPMENT_NULL", "Shipment");

            if (Status != TripStatus.Planned)
                throw new DomainOperationException("Shipments can only be added to planned trips.", "INVALID_TRIP_STATUS", "Trip", Id);

            if (_shipments.Any(s => s.Id == shipment.Id))
                throw new DuplicateShipmentInTripException("Shipment already exists in the trip.", "SHIPMENT_ALREADY_IN_TRIP", "Trip");

            if (shipment.TripId.HasValue)
                throw new TripPlanningException("Shipment is already assigned to a trip.", Id);

            if (TripType == TripType.Pickup && shipment.CurrentStatus != Enums.Shipment.ShipmentStatuses.AssignedToPickUpCarrier)
                throw new TripPlanningException("Pickup trips can only include shipments assigned to a pickup carrier.", Id);

            if (TripType == TripType.Delivery && shipment.CurrentStatus != Enums.Shipment.ShipmentStatuses.InWarehouse)
                throw new TripPlanningException("Delivery trips can only include shipments that are in warehouse.", Id);

            _shipments.Add(shipment);
            TotalShipments = _shipments.Count;
            shipment.TripId = Id;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = operationManagerId.ToString();
        }

    
    }
}
