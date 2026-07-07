using FluentAssertions;
using TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;
using TransitNova.Domain.Tests.TestData;

namespace TransitNova.Domain.Tests.Entities;

public sealed class TripTests
{
    [Fact]
    public void Plan_Should_Create_PlannedTrip_When_PickupShipmentsAreValid()
    {
        var shipment = DomainTestData.CreatePickupAssignedShipment();
        var carrierId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();

        var trip = Trip.Plan(carrierId, warehouseId, TripType.Pickup, [shipment]);

        trip.Id.Should().NotBeEmpty();
        trip.Status.Should().Be(TripStatus.Planned);
        trip.CarrierId.Should().Be(carrierId);
        trip.WarehouseId.Should().Be(warehouseId);
        trip.TotalShipments.Should().Be(1);
        trip.Shipments.Should().ContainSingle().Which.Should().Be(shipment);
        trip.GetDomainEvents().Should().ContainSingle(e => e is TripPlannedDomainEvent);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Plan_Should_ThrowException_When_RequiredIdentifierIsEmpty(bool emptyCarrier, bool emptyWarehouse)
    {
        var carrierId = emptyCarrier ? Guid.Empty : Guid.NewGuid();
        var warehouseId = emptyWarehouse ? Guid.Empty : Guid.NewGuid();

        var act = () => Trip.Plan(carrierId, warehouseId, TripType.Pickup, [DomainTestData.CreatePickupAssignedShipment()]);

        act.Should().Throw<DomainArgumentException>();
    }

    [Fact]
    public void Plan_Should_ThrowException_When_ShipmentCollectionIsEmpty()
    {
        var act = () => Trip.Plan(Guid.NewGuid(), Guid.NewGuid(), TripType.Pickup, []);

        act.Should().Throw<TripPlanningException>();
    }

    [Fact]
    public void Plan_Should_ThrowException_When_ShipmentsContainDuplicates()
    {
        var shipment = DomainTestData.CreatePickupAssignedShipment();

        var act = () => Trip.Plan(Guid.NewGuid(), Guid.NewGuid(), TripType.Pickup, [shipment, shipment]);

        act.Should().Throw<DuplicateShipmentInTripException>();
    }

    [Fact]
    public void Plan_Should_ThrowException_When_ShipmentStateDoesNotMatchTripType()
    {
        var shipment = DomainTestData.CreateShipment();

        var act = () => Trip.Plan(Guid.NewGuid(), Guid.NewGuid(), TripType.Pickup, [shipment]);

        act.Should().Throw<TripPlanningException>();
    }

    [Fact]
    public void StartTrip_Should_ActivateTrip_When_StatusAndTypeAreValid()
    {
        var trip = CreatePickupTrip();
        var managerId = Guid.NewGuid();

        trip.StartTrip(managerId, TripType.Pickup);

        trip.Status.Should().Be(TripStatus.Active);
        trip.StartTime.Should().NotBeNull();
        trip.UpdatedBy.Should().Be(managerId.ToString());
        trip.GetDomainEvents().Should().Contain(e => e is TripStartedDomainEvent);
    }

    [Fact]
    public void StartTrip_Should_ThrowException_When_TypeDoesNotMatch()
    {
        var trip = CreatePickupTrip();

        var act = () => trip.StartTrip(Guid.NewGuid(), TripType.Delivery);

        act.Should().Throw<TripPlanningException>();
    }

    [Fact]
    public void StartTrip_Should_ThrowException_When_TripIsNotPlanned()
    {
        var trip = CreatePickupTrip();
        trip.StartTrip(Guid.NewGuid(), TripType.Pickup);

        var act = () => trip.StartTrip(Guid.NewGuid(), TripType.Pickup);

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("INVALID_TRIP_STATUS");
    }

    [Fact]
    public void Complete_Should_CompleteTrip_When_AssignedCarrierCompletesActiveTrip()
    {
        var (trip, _) = CreateCompletablePickupTrip();

        trip.Complete(trip.CarrierId);

        trip.Status.Should().Be(TripStatus.Completed);
        trip.EndTime.Should().NotBeNull();
        trip.GetDomainEvents().Should().Contain(e => e is TripCompletedDomainEvent);
    }

    [Fact]
    public void Complete_Should_ThrowException_When_PickupShipmentsAreNotInWarehouse()
    {
        var trip = CreatePickupTrip();
        trip.StartTrip(Guid.NewGuid(), TripType.Pickup);

        var act = () => trip.Complete(trip.CarrierId);

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("TRIP_SHIPMENTS_NOT_COMPLETED");
    }

    [Fact]
    public void Complete_Should_ThrowException_When_CarrierDoesNotMatch()
    {
        var trip = CreatePickupTrip();
        trip.StartTrip(Guid.NewGuid(), TripType.Pickup);

        var act = () => trip.Complete(Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("TRIP_CARRIER_MISMATCH");
    }

    [Fact]
    public void Cancel_Should_CancelTrip_When_TripIsNotCompleted()
    {
        var trip = CreatePickupTrip();

        trip.Cancel(Guid.NewGuid());

        trip.Status.Should().Be(TripStatus.Cancelled);
        trip.EndTime.Should().NotBeNull();
        trip.GetDomainEvents().Should().Contain(e => e is TripCancelledDomainEvent);
    }

    [Fact]
    public void Cancel_Should_ThrowException_When_TripIsCompleted()
    {
        var (trip, _) = CreateCompletablePickupTrip();
        trip.Complete(trip.CarrierId);

        var act = () => trip.Cancel(Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("COMPLETED_TRIP_CANNOT_BE_CANCELLED");
    }

    [Fact]
    public void AddShipment_Should_AddShipmentAndAssignTrip_When_ShipmentIsEligible()
    {
        var existing = DomainTestData.CreatePickupAssignedShipment();
        var additional = DomainTestData.CreatePickupAssignedShipment();
        var trip = Trip.Plan(Guid.NewGuid(), Guid.NewGuid(), TripType.Pickup, [existing]);

        trip.AddShipment(additional, Guid.NewGuid());

        trip.TotalShipments.Should().Be(2);
        additional.TripId.Should().Be(trip.Id);
        trip.GetDomainEvents().Should().Contain(e => e is TripShipmentAddedDomainEvent);
    }

    [Fact]
    public void AddShipment_Should_ThrowException_When_ShipmentAlreadyExists()
    {
        var shipment = DomainTestData.CreatePickupAssignedShipment();
        var trip = Trip.Plan(Guid.NewGuid(), Guid.NewGuid(), TripType.Pickup, [shipment]);

        var act = () => trip.AddShipment(shipment, Guid.NewGuid());

        act.Should().Throw<DuplicateShipmentInTripException>();
    }

    private static Trip CreatePickupTrip() =>
        Trip.Plan(Guid.NewGuid(), Guid.NewGuid(), TripType.Pickup, [DomainTestData.CreatePickupAssignedShipment()]);

    private static (Trip Trip, Shipment Shipment) CreateCompletablePickupTrip()
    {
        var carrierId = Guid.NewGuid();
        var shipment = DomainTestData.CreatePickupAssignedShipment();
        var trip = Trip.Plan(carrierId, Guid.NewGuid(), TripType.Pickup, [shipment]);
        trip.StartTrip(Guid.NewGuid(), TripType.Pickup);
        shipment.AssignedAsPickupTrip(trip.Id, carrierId);
        shipment.DeliveredToWarehouse(carrierId);
        return (trip, shipment);
    }
}