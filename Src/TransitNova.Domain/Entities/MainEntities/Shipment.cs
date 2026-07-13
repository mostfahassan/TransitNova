using NUlid;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.Domain.Entities.MainEntities;

public class Shipment : AggregateRoot<Guid>, ISoftDeletable
{
    private readonly List<ShipmentStatus> _shipmentStates = new();
    public IReadOnlyCollection<ShipmentStatus> ShipmentStates => _shipmentStates;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOn { get; private set; }

    public bool IsCancelled { get; private set; }
    public DateTime? CancelledOn { get; private set; }

    public bool IsRejected { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public string? IssueMessage { get; private set; }
    public bool IsIssued { get; private set; }
    public DateTime? IssuedOn { get; private set; }

    public Address DeliveryAddress { get; private set; } = null!;
    public Address PickupAddress { get; private set; } = null!;
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

    public Guid? HandlerId { get; private set; }
    public virtual OperationManagerProfile? HandledBy { get; private set; }

    public virtual Trip? Trip { get; private set; }
    public Guid? TripId { get; set; }

    public Guid PaymentId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }

    private Shipment() { }

    private Shipment(
        Guid senderId,
        ReceiverProfile receiver,
        PackageSpecification packageSpecification,
        Currency currency,
        DateTime? pickUpDate,
        Address deliveryAddress,
        Address pickupAddress,
        enShipmentType shipmentType,
        TransportationMode mode,
        PaymentMethod paymentMethod)
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

        TrackingNumber = GenerateTrackingNumber();
        CurrentStatus = ShipmentStatuses.Pending;

        CreatedBy = senderId.ToString();
        CurrentState = true;
        PaymentMethod = paymentMethod;
    }

    public static Shipment Create(
        Guid senderId,
        ReceiverProfile receiver,
        PackageSpecification packageSpecification,
        Currency currency,
        DateTime? pickUpDate,
        Address deliveryAddress,
        Address pickupAddress,
        enShipmentType shipmentType,
        TransportationMode mode,
        PaymentMethod paymentMethod)
    {
        var shipment = new Shipment(
            senderId,
            receiver,
            packageSpecification,
            currency,
            pickUpDate,
            deliveryAddress,
            pickupAddress,
            shipmentType,
            mode,
            paymentMethod);

        shipment._shipmentStates.Add(shipment.CreateHistory(ShipmentStatuses.Pending));

        shipment.RaiseDomainEvent(
            new ShipmentCreatedDomainEvent(shipment.Id, shipment.TrackingNumber));

        return shipment;
    }

    private static string GenerateTrackingNumber()
        => $"TRK-{Ulid.NewUlid().ToString().ToUpperInvariant()}";

    private ShipmentStatus CreateHistory(ShipmentStatuses statusType, Guid? carrierId = null)
    {
        var current = _shipmentStates.FirstOrDefault(x => x.CurrentState);
        current?.CurrentState = false;

        return new ShipmentStatus
        {
            ShipmentId = Id,
            StatusType = statusType,
            CurrentState = true,
            CarrierId = carrierId
        };
    }

    private void ChangeStatus(ShipmentStatuses newStatus, Guid? carrierId = null, Guid? handledById = null)
    {
        CurrentStatus = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (handledById.HasValue)
        {
            HandlerId = handledById;
            UpdatedBy = handledById.Value.ToString();
        }

        _shipmentStates.Add(CreateHistory(newStatus, carrierId));
    }

    public void UpdateShipmentDetails(
        Guid? receiverId,
        Address? deliveryAddress,
        Address? pickupAddress,
        TransportationMode? mode,
        enShipmentType? shipmentType,
        PackageSpecification? packageSpecification,
        decimal? newCost = null,
        DateTime? updatedDate = null)
    {
        EnsureNotCancelledOrDeleted();
        EnsureNotFinalState();

        ReceiverId = receiverId ?? ReceiverId;
        DeliveryAddress = deliveryAddress ?? DeliveryAddress;
        PickupAddress = pickupAddress ?? PickupAddress;
        Mode = mode ?? Mode;
        ShipmentType = shipmentType ?? ShipmentType;
        PackageSpecification = packageSpecification ?? PackageSpecification;

        if (newCost.HasValue)
            ShipmentCost = newCost.Value;

        if (updatedDate.HasValue)
            EstimatedDeliveryDate = updatedDate.Value;

        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new ShipmentUpdatedDomainEvent(Id, TrackingNumber));
    }

    public void ApproveShipment(Guid handlerId)
    {
        EnsurePending();
        ChangeStatus(ShipmentStatuses.Approved, handledById: handlerId);
        RaiseDomainEvent(new ShipmentApprovedDomainEvent(Id, TrackingNumber));
    }

    public void RejectShipment(Guid handlerId, string rejectionReason)
    {
        EnsureNotCancelledOrDeleted();
        EnsurePending();
        IsRejected = true;
        RejectedAt = DateTime.UtcNow;
        RejectionReason = rejectionReason;
        ChangeStatus(ShipmentStatuses.Rejected, handledById: handlerId);
        RaiseDomainEvent(new ShipmentRejectedDomainEvent(Id, TrackingNumber, rejectionReason));
    }

    public void CancelShipment()
    {
        EnsureNotFinalState();
        IsCancelled = true;
        CancelledOn = DateTime.UtcNow;
        ChangeStatus(ShipmentStatuses.Cancelled);
        RaiseDomainEvent(new ShipmentCancelledDomainEvent(Id, TrackingNumber));
    }

    public void IssueShipment(string issueMessage)
    {
        EnsureDelivered();
        IsIssued = true;
        IssueMessage = issueMessage;
        IssuedOn = DateTime.UtcNow;
        ChangeStatus(ShipmentStatuses.Issue);
        RaiseDomainEvent(new ShipmentIssuedDomainEvent(Id, TrackingNumber, issueMessage));
    }

    public void DeleteShipment()
    {
        if (IsCancelled || IsDeleted)
            throw new DomainOperationException("Already cancelled or deleted.", "ALREADY_CANCELLED_OR_DELETED", typeof(Shipment).Name, Id);

        if (CurrentStatus is not ShipmentStatuses.Delivered)
            throw new InvalidShipmentStateException(Id, "Delivered", CurrentStatus.ToString());

        IsDeleted = true;
        DeletedOn = DateTime.UtcNow;
        ChangeStatus(ShipmentStatuses.Deleted);
        RaiseDomainEvent(new ShipmentDeletedDomainEvent(Id, TrackingNumber));
    }

    public void AssignedAsDeliveryTrip(Guid tripId, Guid carrierId)
    {
        TripId = tripId;
        if (CurrentStatus is ShipmentStatuses.Cancelled or ShipmentStatuses.Rejected)
            throw new DomainOperationException("Shipment Is Being Rejected Or Cancelled,Cant Assigned For Trip");

        ChangeStatus(ShipmentStatuses.OutForDelivery, carrierId);
        RaiseDomainEvent(new ShipmentAssignedToCarrierDomainEvent(Id, TrackingNumber, ShipmentStatuses.OutForDelivery));
    }

    public void AssignedAsPickupTrip(Guid tripId, Guid carrierId)
    {
        TripId = tripId;
        if (CurrentStatus is ShipmentStatuses.Cancelled or ShipmentStatuses.Rejected)
            throw new DomainOperationException("Shipment Is Being Rejected Or Cancelled,Cant Assigned For Trip");

        ChangeStatus(ShipmentStatuses.OutForPickup, carrierId);
        RaiseDomainEvent(new ShipmentAssignedToCarrierDomainEvent(Id, TrackingNumber, ShipmentStatuses.OutForPickup));
    }

    public void Delivered(Guid carrierId)
    {
        if (CurrentStatus != ShipmentStatuses.OutForDelivery)
            throw new ShipmentNotAssignedException(Id, carrierId);

        ActualDeliveryDate = DateTime.UtcNow;
        ChangeStatus(ShipmentStatuses.Delivered, carrierId);
        RaiseDomainEvent(new ShipmentDeliveredDomainEvent(Id, TrackingNumber));
    }

    public void PickedUp(Guid carrierId)
    {
        if (CurrentStatus != ShipmentStatuses.OutForPickup)
            throw new ShipmentNotAssignedException(Id, carrierId);

        PickupDate = DateTime.UtcNow;
        ChangeStatus(ShipmentStatuses.PickedUp, carrierId);
        RaiseDomainEvent(new ShipmentPickedUpDomainEvent(Id, TrackingNumber));
    }

    public void DeliveredToWarehouse(Guid carrierId)
    {
        if (CurrentStatus is not (ShipmentStatuses.OutForPickup or ShipmentStatuses.PickedUp))
            throw new ShipmentNotAssignedException(Id, carrierId);

        PickupDate ??= DateTime.UtcNow;
        ChangeStatus(ShipmentStatuses.InWarehouse, carrierId);
        RaiseDomainEvent(new ShipmentDeliveredToWarehouseDomainEvent(Id, TrackingNumber));
    }

    public void AssignToCarrier(ShipmentStatuses newStatus, Guid carrierId, Guid handlerId)
    {
        if (CurrentStatus is ShipmentStatuses.Cancelled or ShipmentStatuses.Rejected or ShipmentStatuses.Delivered)
            throw new DomainOperationException("Invalid state", "INVALID_STATE", nameof(Shipment), Id);

        if (newStatus is not (ShipmentStatuses.AssignedToPickUpCarrier or ShipmentStatuses.AssignedToDeliveryCarrier))
            throw new DomainOperationException("Invalid assignment", "INVALID_ASSIGNMENT", nameof(Shipment), Id);

        ChangeStatus(newStatus, carrierId, handlerId);

        if (newStatus == ShipmentStatuses.AssignedToPickUpCarrier)
            PickupDate = DateTime.UtcNow;

        RaiseDomainEvent(new ShipmentAssignedToCarrierDomainEvent(Id, TrackingNumber, newStatus));
    }

    private void EnsurePending()
    {
        if (CurrentStatus != ShipmentStatuses.Pending)
            throw new InvalidShipmentStateException(Id, "Pending", CurrentStatus.ToString());
    }

    private void EnsureDelivered()
    {
        if (CurrentStatus != ShipmentStatuses.Delivered)
            throw new InvalidShipmentStateException(Id, "Delivered", CurrentStatus.ToString());
    }

    private void EnsureNotCancelledOrDeleted()
    {
        if (IsCancelled || IsDeleted)
            throw new DomainOperationException("Invalid operation", "CANCELLED_OR_DELETED", nameof(Shipment), Id);
    }

    private void EnsureNotFinalState()
    {
        if (CurrentStatus is ShipmentStatuses.Delivered or ShipmentStatuses.Rejected)
            throw new InvalidShipmentStateException(Id, "NotFinalState", CurrentStatus.ToString());
    }

    public void SetShipmentCost(decimal cost, DateTime estimatedDeliveryDate)
    {
        if (cost < 0)
            throw new DomainOperationException("Shipment cost cannot be negative.", "NEGATIVE_COST", nameof(Shipment), Id);

        ShipmentCost = cost;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }

    public void SetPaymentId(Guid paymentId)
    {
        PaymentId = paymentId;
    }
}