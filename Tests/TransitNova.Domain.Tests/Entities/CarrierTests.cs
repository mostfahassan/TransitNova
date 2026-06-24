using FluentAssertions;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Trip;
using TransitNova.Domain.Tests.TestData;

namespace TransitNova.Domain.Tests.Entities;

public sealed class CarrierTests
{
    [Fact]
    public void CreateCarrier_Should_Create_AvailableCarrierAndEvent_When_DataIsValid()
    {
        var userId = Guid.NewGuid();

        var carrier = CreateCarrier(userId);

        carrier.Id.Should().NotBeEmpty();
        carrier.AppUserId.Should().Be(userId);
        carrier.Status.Should().Be(CarrierStatus.Available);
        carrier.Code.Should().StartWith("CR-");
        carrier.CurrentState.Should().BeTrue();
        carrier.GetDomainEvents().Should().ContainSingle(e => e is CarrierCreatedDomainEvent);
    }

    [Fact]
    public void AddAdditionalData_Should_SetContractAndOperationalData_When_NotPreviouslyAdded()
    {
        var carrier = CreateCarrier();
        var userId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        carrier.AddAdditionalData(userId, "LIC-10", 5, 15m, 7, startDate, 2, warehouseId);

        carrier.HasAdditionalInfo.Should().BeTrue();
        carrier.LicenseNumber.Should().Be("LIC-10");
        carrier.MaxDailyShipments.Should().Be(5);
        carrier.DefaultCostPerKg.Should().Be(15m);
        carrier.YearsOfExperience.Should().Be(7);
        carrier.ContractStartDate.Should().Be(startDate);
        carrier.ContractEndDate.Should().Be(startDate.AddYears(2));
        carrier.HomeWarehouseId.Should().Be(warehouseId);
        carrier.UpdatedBy.Should().Be(userId.ToString());
        carrier.GetDomainEvents().Should().Contain(e => e is CarrierAdditionalInfoAddedDomainEvent);
    }

    [Fact]
    public void AddAdditionalData_Should_ThrowException_When_AdditionalDataAlreadyExists()
    {
        var carrier = CreateCarrierWithAdditionalData();

        var act = () => carrier.AddAdditionalData(Guid.NewGuid(), "LIC-20", 3, 10m, 2, DateTime.UtcNow, 1, null);

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("CARRIER_ADDITIONAL_INFO_EXISTS");
    }

    [Fact]
    public void UpdateProfile_Should_TrimProvidedValuesAndRetainNullValues_When_Called()
    {
        var carrier = CreateCarrier();
        var userId = Guid.NewGuid();

        carrier.UpdateProfile(userId, " New ", null, " 0111 ", " New address ");

        carrier.FirstName.Should().Be("New");
        carrier.LastName.Should().Be("Carrier");
        carrier.PhoneNumber.Should().Be("0111");
        carrier.Address.Should().Be("New address");
        carrier.UpdatedBy.Should().Be(userId.ToString());
        carrier.GetDomainEvents().Should().Contain(e => e is CarrierProfileUpdatedDomainEvent);
    }

    [Fact]
    public void AssignToPickup_Should_IncrementAssignedCountAndChangeStatus_When_CarrierIsAvailable()
    {
        var carrier = CreateCarrierWithAdditionalData();
        var managerId = Guid.NewGuid();

        carrier.AssignToPickup(managerId);

        carrier.AssignedShipmentsCount.Should().Be(1);
        carrier.Status.Should().Be(CarrierStatus.AssignedToPickUpShipment);
        carrier.HandlerId.Should().Be(managerId);
        carrier.GetDomainEvents().Should().Contain(e => e is CarrierAssignedToPickupDomainEvent);
    }

    [Fact]
    public void AssignToDeliver_Should_IncrementAssignedCountAndChangeStatus_When_CarrierIsAvailable()
    {
        var carrier = CreateCarrierWithAdditionalData();

        carrier.AssignToDeliver(Guid.NewGuid());

        carrier.AssignedShipmentsCount.Should().Be(1);
        carrier.Status.Should().Be(CarrierStatus.AssignedToDeliveryShipment);
        carrier.GetDomainEvents().Should().Contain(e => e is CarrierAssignedToDeliveryDomainEvent);
    }

    [Fact]
    public void AssignToPickup_Should_ThrowException_When_CarrierIsBusy()
    {
        var carrier = CreateCarrierWithAdditionalData();
        carrier.AssignToPickup(Guid.NewGuid());

        var act = () => carrier.AssignToPickup(Guid.NewGuid());

        act.Should().Throw<InvalidCarrierStatusException>();
    }

    [Fact]
    public void CompleteShipment_Should_IncrementCompletedAndRestoreAvailability_When_LastAssignmentCompletes()
    {
        var carrier = CreateCarrierWithAdditionalData();
        carrier.AssignToPickup(Guid.NewGuid());

        carrier.CompleteShipment();

        carrier.AssignedShipmentsCount.Should().Be(0);
        carrier.CompletedShipmentsCount.Should().Be(1);
        carrier.Status.Should().Be(CarrierStatus.Available);
        carrier.GetDomainEvents().Should().Contain(e => e is CarrierShipmentCompletedDomainEvent);
    }

    [Fact]
    public void CompleteShipment_Should_ThrowException_When_NoShipmentIsAssigned()
    {
        var carrier = CreateCarrierWithAdditionalData();

        var act = carrier.CompleteShipment;

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("NO_ASSIGNED_SHIPMENTS");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void AddRating_Should_ThrowException_When_RatingIsOutsideRange(int rating)
    {
        var carrier = CreateCarrier();

        var act = () => carrier.AddRating(rating);

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("CARRIER_RATING_INVALID");
    }

    [Fact]
    public void AddRating_Should_CalculateRunningAverage_When_MultipleRatingsAreAdded()
    {
        var carrier = CreateCarrier();

        carrier.AddRating(5);
        carrier.AddRating(3);

        carrier.AverageRating.Should().Be(4m);
        carrier.TotalRatings.Should().Be(2);
        carrier.GetDomainEvents().OfType<CarrierRatedDomainEvent>().Should().HaveCount(2);
    }

    [Fact]
    public void AssignToTrip_Should_ChangeStatus_When_AssignedTripBelongsToCarrier()
    {
        var carrier = CreateCarrierWithAdditionalData();
        var shipment = DomainTestData.CreatePickupAssignedShipment();
        var trip = Trip.Plan(carrier.Id, Guid.NewGuid(), TripType.Pickup, [shipment]);
        carrier.Trips.Add(trip);
        carrier.AssignToPickup(Guid.NewGuid());

        carrier.AssignToTrip(Guid.NewGuid(), trip.Id);

        carrier.TripId.Should().Be(trip.Id);
        carrier.Status.Should().Be(CarrierStatus.OnTrip);
        carrier.GetDomainEvents().Should().Contain(e => e is CarrierTripStartedDomainEvent);
    }

    [Fact]
    public void AssignToTrip_Should_ThrowException_When_TripsAreNotLoaded()
    {
        var carrier = CreateCarrierWithAdditionalData();
        carrier.AssignToPickup(Guid.NewGuid());

        var act = () => carrier.AssignToTrip(Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("TRIPS_NOT_LOADED");
    }

    private static Carrier CreateCarrier(Guid? userId = null) =>
        Carrier.Create(userId ?? Guid.NewGuid(), "Test", "Carrier", "carrier@example.com", "0100", "Cairo", 1);

    private static Carrier CreateCarrierWithAdditionalData()
    {
        var carrier = CreateCarrier();
        carrier.AddAdditionalData(Guid.NewGuid(), "LIC-1", 3, 10m, 4, DateTime.UtcNow.Date, 1, null);
        return carrier;
    }
}
