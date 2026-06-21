using FluentAssertions;
using TransitNova.Domain.Contracts.DomainEvents.Events.ShipmentEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Tests.TestData;

namespace TransitNova.Domain.Tests.Entities;

public sealed class ShipmentTests
{
    [Fact]
    public void CreateShipment_Should_Create_PendingShipment_When_DataIsValid()
    {
        var senderId = Guid.NewGuid();
        var receiver = DomainTestData.CreateReceiver(senderId);
        var package = new PackageSpecification(12m, 2m, 3m, 4m);

        var shipment = Shipment.Create(
            senderId,
            receiver,
            package,
            Currency.USD,
            DateTime.UtcNow.AddDays(1),
            "Alexandria",
            "Cairo",
            enShipmentType.Express,
            TransportationMode.Land,
            Guid.NewGuid(),
            500m,
            DateTime.UtcNow.AddDays(2));

        shipment.Id.Should().NotBeEmpty();
        shipment.SenderId.Should().Be(senderId);
        shipment.ReceiverId.Should().Be(receiver.Id);
        shipment.PackageSpecification.Should().Be(package);
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Pending);
        shipment.TrackingNumber.Should().StartWith("TRK-");
        shipment.ShipmentStates.Should().ContainSingle(s => s.StatusType == ShipmentStatuses.Pending && s.CurrentState);
        shipment.GetDomainEvents().Should().ContainSingle(e => e is ShipmentCreatedDomainEvent);
    }

    [Fact]
    public void UpdateShipmentDetails_Should_UpdateOnlyProvidedValues_When_ShipmentIsMutable()
    {
        var shipment = DomainTestData.CreateShipment();
        var package = new PackageSpecification(25m, 5m, 6m, 7m);
        var deliveryDate = DateTime.UtcNow.AddDays(10);

        shipment.UpdateShipmentDetails(null, "New delivery", null, TransportationMode.Air, null, package, 900m, deliveryDate);

        shipment.DeliveryAddress.Should().Be("New delivery");
        shipment.PickupAddress.Should().Be("Pickup address");
        shipment.Mode.Should().Be(TransportationMode.Air);
        shipment.PackageSpecification.Should().Be(package);
        shipment.ShipmentCost.Should().Be(900m);
        shipment.EstimatedDeliveryDate.Should().Be(deliveryDate);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentUpdatedDomainEvent);
    }

    [Fact]
    public void ApproveShipment_Should_SetApprovedStatusAndHandler_When_ShipmentIsPending()
    {
        var shipment = DomainTestData.CreateShipment();
        var handlerId = Guid.NewGuid();

        shipment.ApproveShipment(handlerId);

        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Approved);
        shipment.HandledById.Should().Be(handlerId);
        shipment.ShipmentStates.Should().HaveCount(2);
        shipment.ShipmentStates.Should().ContainSingle(s => s.CurrentState && s.StatusType == ShipmentStatuses.Approved);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentApprovedDomainEvent);
    }

    [Fact]
    public void ApproveShipment_Should_ThrowException_When_ShipmentIsNotPending()
    {
        var shipment = DomainTestData.CreateShipment();
        shipment.ApproveShipment(Guid.NewGuid());

        var act = () => shipment.ApproveShipment(Guid.NewGuid());

        act.Should().Throw<InvalidShipmentStateException>();
    }

    [Fact]
    public void RejectShipment_Should_RecordReasonAndHandler_When_ShipmentIsPending()
    {
        var shipment = DomainTestData.CreateShipment();
        var handlerId = Guid.NewGuid();

        shipment.RejectShipment(handlerId, "Unsupported contents");

        shipment.IsRejected.Should().BeTrue();
        shipment.RejectionReason.Should().Be("Unsupported contents");
        shipment.RejectedAt.Should().NotBeNull();
        shipment.HandledById.Should().Be(handlerId);
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Rejected);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentRejectedDomainEvent);
    }

    [Fact]
    public void RejectShipment_Should_ThrowException_When_ShipmentIsAlreadyApproved()
    {
        var shipment = DomainTestData.CreateShipment();
        shipment.ApproveShipment(Guid.NewGuid());

        var act = () => shipment.RejectShipment(Guid.NewGuid(), "Late");

        act.Should().Throw<InvalidShipmentStateException>();
    }

    [Fact]
    public void CancelShipment_Should_CancelShipment_When_ShipmentIsNotFinal()
    {
        var shipment = DomainTestData.CreateShipment();

        shipment.CancelShipment();

        shipment.IsCancelled.Should().BeTrue();
        shipment.CancelledOn.Should().NotBeNull();
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Cancelled);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentCancelledDomainEvent);
    }

    [Fact]
    public void CancelShipment_Should_ThrowException_When_ShipmentIsDelivered()
    {
        var shipment = DomainTestData.CreateDeliveredShipment();

        var act = shipment.CancelShipment;

        act.Should().Throw<InvalidShipmentStateException>();
    }

    [Theory]
    [InlineData(ShipmentStatuses.AssignedToPickUpCarrier)]
    [InlineData(ShipmentStatuses.AssignedToDeliveryCarrier)]
    public void AssignToCarrier_Should_SetStatusAndHistory_When_AssignmentStatusIsValid(ShipmentStatuses status)
    {
        var shipment = DomainTestData.CreateShipment();
        var carrierId = Guid.NewGuid();
        var handlerId = Guid.NewGuid();

        shipment.AssignToCarrier(status, carrierId, handlerId);

        shipment.CurrentStatus.Should().Be(status);
        shipment.HandledById.Should().Be(handlerId);
        shipment.ShipmentStates.Should().ContainSingle(s => s.CurrentState && s.CarrierId == carrierId && s.StatusType == status);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentAssignedToCarrierDomainEvent);
    }

    [Fact]
    public void AssignToCarrier_Should_ThrowException_When_AssignmentStatusIsInvalid()
    {
        var shipment = DomainTestData.CreateShipment();

        var act = () => shipment.AssignToCarrier(ShipmentStatuses.Delivered, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("INVALID_ASSIGNMENT");
    }

    [Fact]
    public void AssignToCarrier_Should_ThrowException_When_ShipmentIsCancelled()
    {
        var shipment = DomainTestData.CreateShipment();
        shipment.CancelShipment();

        var act = () => shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("INVALID_STATE");
    }

    [Fact]
    public void AssignedAsPickupTrip_Should_SetTripAndOutForPickup_When_ShipmentIsAssignable()
    {
        var shipment = DomainTestData.CreatePickupAssignedShipment();
        var tripId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();

        shipment.AssignedAsPickUpTrip(tripId, carrierId);

        shipment.TripId.Should().Be(tripId);
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.OutForPickup);
    }

    [Fact]
    public void DeliveredToWarehouse_Should_SetInWarehouseAndPickupDate_When_OutForPickup()
    {
        var shipment = DomainTestData.CreatePickupAssignedShipment();
        shipment.AssignedAsPickUpTrip(Guid.NewGuid(), Guid.NewGuid());

        shipment.DeliveredToWarehouse(Guid.NewGuid());

        shipment.CurrentStatus.Should().Be(ShipmentStatuses.InWarehouse);
        shipment.PickupDate.Should().NotBeNull();
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentDeliveredToWarehouseDomainEvent);
    }

    [Fact]
    public void DeliveredToWarehouse_Should_ThrowException_When_NotOutForPickup()
    {
        var shipment = DomainTestData.CreateShipment();

        var act = () => shipment.DeliveredToWarehouse(Guid.NewGuid());

        act.Should().Throw<ShipmentNotAssignedException>();
    }

    [Fact]
    public void Delivered_Should_SetDeliveredAndActualDate_When_OutForDelivery()
    {
        var shipment = DomainTestData.CreateInWarehouseShipment();
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToDeliveryCarrier, Guid.NewGuid(), Guid.NewGuid());
        shipment.AssignedAsDeliveryTrip(Guid.NewGuid(), Guid.NewGuid());

        shipment.Delivered(Guid.NewGuid());

        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Delivered);
        shipment.ActualDeliveryDate.Should().NotBeNull();
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentDeliveredDomainEvent);
    }

    [Fact]
    public void Delivered_Should_ThrowException_When_NotOutForDelivery()
    {
        var shipment = DomainTestData.CreateShipment();

        var act = () => shipment.Delivered(Guid.NewGuid());

        act.Should().Throw<ShipmentNotAssignedException>();
    }

    [Fact]
    public void IssueShipment_Should_RecordIssue_When_ShipmentIsDelivered()
    {
        var shipment = DomainTestData.CreateDeliveredShipment();

        shipment.IssueShipment("Damaged package");

        shipment.IsIssued.Should().BeTrue();
        shipment.IssueMessage.Should().Be("Damaged package");
        shipment.IssuedOn.Should().NotBeNull();
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Issue);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentIssuedDomainEvent);
    }

    [Fact]
    public void IssueShipment_Should_ThrowException_When_ShipmentIsNotDelivered()
    {
        var shipment = DomainTestData.CreateShipment();

        var act = () => shipment.IssueShipment("Missing item");

        act.Should().Throw<InvalidShipmentStateException>();
    }

    [Fact]
    public void DeleteShipment_Should_SoftDelete_When_ShipmentIsDelivered()
    {
        var shipment = DomainTestData.CreateDeliveredShipment();

        shipment.DeleteShipment();

        shipment.IsDeleted.Should().BeTrue();
        shipment.DeletedOn.Should().NotBeNull();
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Deleted);
        shipment.GetDomainEvents().Should().Contain(e => e is ShipmentDeletedDomainEvent);
    }

    [Fact]
    public void DeleteShipment_Should_ThrowException_When_ShipmentIsNotDelivered()
    {
        var shipment = DomainTestData.CreateShipment();

        var act = shipment.DeleteShipment;

        act.Should().Throw<InvalidShipmentStateException>();
    }

    [Fact]
    public void UpdateShipmentDetails_Should_ThrowException_When_ShipmentIsRejected()
    {
        var shipment = DomainTestData.CreateShipment();
        shipment.RejectShipment(Guid.NewGuid(), "Rejected");

        var act = () => shipment.UpdateShipmentDetails(null, "Address", null, null, null, null);

        act.Should().Throw<InvalidShipmentStateException>();
    }
}
