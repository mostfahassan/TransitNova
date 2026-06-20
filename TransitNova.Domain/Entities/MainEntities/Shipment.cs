using NUlid;
using TransitNova.BusinessLayer.Interfaces;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Shipment;


namespace TransitNova.Domain.Entities.MainEntities
{
    public class Shipment : BaseEntity<Guid>, ISoftDeletable
    {
        private readonly List<ShipmentStatus> _shipmentStates = new();

        //======== Deletion ========//
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }

        //======= Cancellation =======//
        public bool IsCancelled { get; private set; } = false;
        public DateTime? CancelledOn { get; private set; }

        //======= Rejection =======//
        public bool IsRejected { get; private set; } = false;
        public DateTime? RejectedAt { get; private set; }
        public string? RejectionReason { get; private set; }

        //======= Issue Tracking =======//
        public string? IssueMessage { get; private set; }
        public bool IsIssued { get; private set; } = false;
        public DateTime? IssuedOn { get; private set; }

        public string DeliveryAddress { get; private set; } = string.Empty;
        public string PickupAddress { get; private set; } = string.Empty;
        public decimal ShipmentCost { get; private set; }

        public PackageSpecification PackageSpecification { get; private set; } = null!;
        public Currency Currency { get; private set; }
        public ShipmentStatuses CurrentStatus { get; private set; }
        public enShipmentType ShipmentType { get; private set; }
        public TransportationMode Mode { get; private set; }

        public DateTime? EstimatedDeliveryDate { get; private set; }
        public DateTime? PickupDate { get; private set; }
        public DateTime? ActualDeliveryDate { get; private set; }

        public string TrackingNumber { get; private set; } = null!;
        public byte[] RowVersion { get; private set; } = default!;

        public Guid ReceiverId { get; private set; }
        public virtual ReceiverProfile Receiver { get; private set; } = null!;

        public Guid SenderId { get; private set; }
        public virtual UserProfile Sender { get; private set; } = null!;

        public Guid? HandledById { get; private set; }
        public virtual OperationManagerProfile? HandledBy { get; private set; }

        public virtual Payment? Payment { get; private set; }

        public int? PackageBundleId { get; private set; }
        public virtual Bundle? PackageBundle { get; private set; }

        public IReadOnlyCollection<ShipmentStatus> ShipmentStates => _shipmentStates;

        public virtual Trip? Trip { get; private set; }
        public Guid? TripId { get;  set; }

        private Shipment()
        {
        }

        private Shipment(Guid senderId, ReceiverProfile receiver, PackageSpecification packageSpecification, Currency currency, DateTime? pickUpDate, string deliveryAddress, string pickupAddress, enShipmentType shipmentType, TransportationMode mode, int? packageBundleId, decimal shipmentCost, DateTime deliveryDate)
        {
            Id = Guid.CreateVersion7();
            SenderId = senderId;
            ReceiverId = receiver.Id;
            Receiver = receiver;
            PackageSpecification = packageSpecification;
            Currency = currency;
            PickupDate = pickUpDate;
            Mode = mode;
            ShipmentType = shipmentType;
            DeliveryAddress = deliveryAddress;
            PickupAddress = pickupAddress;
            PackageBundleId = packageBundleId;
            ShipmentCost = shipmentCost;
            EstimatedDeliveryDate = deliveryDate;
            CurrentStatus = ShipmentStatuses.Pending;
            TrackingNumber = GenerateTrackingNumber();
            CreatedAt = DateTime.UtcNow;
            CreatedBy = senderId.ToString();
            CurrentState = true;
            var initialState = CreateHistory(ShipmentStatuses.Pending);
            _shipmentStates.Add(initialState);
        }

        public static Shipment Create(Guid senderId, ReceiverProfile receiver, PackageSpecification packageSpecification, Currency currency, DateTime? pickUpDate, string deliveryAddress, string pickupAddress, enShipmentType shipmentType, TransportationMode mode, int? packageBundleId, decimal shipmentCost, DateTime deliveryDate)
            => new (senderId, receiver, packageSpecification, currency, pickUpDate, deliveryAddress, pickupAddress, shipmentType, mode, packageBundleId, shipmentCost, deliveryDate);

      
        private static string GenerateTrackingNumber()
            => $"TRK-{Ulid.NewUlid().ToString().ToUpperInvariant()}";

        private ShipmentStatus CreateHistory(ShipmentStatuses statusType, Guid? carrierId = null)
        {
            var currentState = _shipmentStates.FirstOrDefault(x => x.CurrentState);

            currentState?.CurrentState = false;

            return new ShipmentStatus
            {
                ShipmentId = Id,
                StatusType = statusType,
                CreatedAt = DateTime.UtcNow,
                CurrentState = true,
                CarrierId = carrierId
            };
        }

        public  void UpdateShipmentDetails(Guid? receiverId, string? deliveryAddress, string? pickupAddress,TransportationMode? mode, enShipmentType? shipmentType, PackageSpecification? packageSpecification, decimal? newCost = null,DateTime? updatedDate = null)
        {
            if (IsCancelled || IsDeleted)
                throw new DomainOperationException("Cannot update a cancelled or deleted shipment.", "CANNOT_UPDATE_CANCELLED_OR_DELETED", "Shipment", Id);
            if (CurrentStatus is ShipmentStatuses.Delivered or ShipmentStatuses.Rejected)
                throw new InvalidShipmentStateException(Id, "NotDeliveredOrRejected", CurrentStatus.ToString());

            ReceiverId = receiverId ?? ReceiverId;
            DeliveryAddress = deliveryAddress ?? DeliveryAddress;
            PickupAddress = pickupAddress ?? PickupAddress;
            Mode = mode ?? Mode;
            ShipmentType = shipmentType ?? ShipmentType;
            PackageSpecification = packageSpecification ?? PackageSpecification;
            UpdatedAt = DateTime.UtcNow;
            if (newCost.HasValue)
            {
                ShipmentCost = newCost.Value;
            }
            if (updatedDate.HasValue)
            {
                EstimatedDeliveryDate = updatedDate.Value;
            }

        }
        public void ChangeStatus(ShipmentStatuses newStatus, Guid? carrierId = null, Guid? handledById = null)
        {
            CurrentStatus = newStatus;
            UpdatedAt = DateTime.UtcNow;
            if (handledById.HasValue)
            {
                HandledById = handledById;
                UpdatedBy = handledById.Value.ToString();
            }
            _shipmentStates.Add(CreateHistory(newStatus, carrierId));
        }

        public void CancelShipment()
        {
            if (IsCancelled)
                throw new DomainOperationException("Already cancelled.", "ALREADY_CANCELLED", typeof(Shipment).Name, Id);

            if (CurrentStatus is ShipmentStatuses.Delivered or ShipmentStatuses.Rejected)
                throw new InvalidShipmentStateException(Id, "Cancellable", CurrentStatus.ToString());

            IsCancelled = true;
            CancelledOn = DateTime.UtcNow;
            ChangeStatus(ShipmentStatuses.Cancelled);
        }
        public void IssueShipment(string issueMessage)
        {
            if (IsCancelled)
                throw new DomainOperationException("Already cancelled.", "ALREADY_CANCELLED", typeof(Shipment).Name, Id);

            if (CurrentStatus is not ShipmentStatuses.Delivered )
                throw new InvalidShipmentStateException(Id, "Delivered", CurrentStatus.ToString());
            IssueMessage = issueMessage;
            IsIssued = true;
            IssuedOn = DateTime.UtcNow;
            ChangeStatus(ShipmentStatuses.Issue);
        }
        public void DeleteShipment()
        {
            if (IsCancelled || IsDeleted)
                throw new DomainOperationException("Already cancelled or deleted.", "ALREADY_CANCELLED_OR_DELETED", typeof(Shipment).Name, Id);

            if (CurrentStatus is not ShipmentStatuses.Delivered )
                throw new InvalidShipmentStateException(Id, "Delivered", CurrentStatus.ToString());

            IsDeleted = true;
            DeletedOn = DateTime.UtcNow;
            ChangeStatus(ShipmentStatuses.Deleted);
        }

        public void Delivered(Guid carrierId)
        {
            if (CurrentStatus != ShipmentStatuses.OutForDelivery)
                throw new ShipmentNotAssignedException(Id, carrierId);

            ActualDeliveryDate = DateTime.UtcNow;
            ChangeStatus(ShipmentStatuses.Delivered, carrierId);
        }

        public void DeliveredToWarehouse(Guid carrierId)
        {
            if (CurrentStatus != ShipmentStatuses.OutForPickup)
                throw new ShipmentNotAssignedException(Id, carrierId);

            PickupDate = DateTime.UtcNow;
            ChangeStatus(ShipmentStatuses.InWarehouse, carrierId);
        }

        public void ApproveShipment(Guid handlerId)
        {
            if (CurrentStatus != ShipmentStatuses.Pending)
                throw new InvalidShipmentStateException(Id, "Pending", CurrentStatus.ToString());

            ChangeStatus(ShipmentStatuses.Approved, handledById: handlerId);
        }

        public void RejectShipment(Guid handlerId, string rejectionReason)
        {
            if (IsRejected || IsCancelled)
                throw new DomainOperationException("Shipment is already rejected or cancelled.", "ALREADY_REJECTED_OR_CANCELLED", typeof(Shipment).Name, Id);

            if (CurrentStatus != ShipmentStatuses.Pending)
                throw new InvalidShipmentStateException(Id, "Pending", CurrentStatus.ToString());

            IsRejected = true;
            RejectedAt = DateTime.UtcNow;
            RejectionReason = rejectionReason;

            ChangeStatus(ShipmentStatuses.Rejected, handledById: handlerId);
        }

        public void AssignToCarrier(ShipmentStatuses newStatus, Guid carrierId, Guid handlerId)
        {
            if (CurrentStatus is ShipmentStatuses.Cancelled or ShipmentStatuses.Rejected or ShipmentStatuses.Delivered)
                throw new DomainOperationException("Shipment cannot be assigned in its current state.", "CANNOT_ASSIGN_IN_CURRENT_STATE", typeof(Shipment).Name, Id);

            if (newStatus != ShipmentStatuses.AssignedToPickUpCarrier && newStatus != ShipmentStatuses.AssignedToDeliveryCarrier)
                throw new DomainOperationException("Invalid assignment status.", "INVALID_ASSIGNMENT_STATUS", typeof(Shipment).Name, Id);

            ChangeStatus(newStatus, carrierId, handlerId);

            if (newStatus == ShipmentStatuses.AssignedToPickUpCarrier)
                PickupDate = DateTime.UtcNow;
        }
    }
}
