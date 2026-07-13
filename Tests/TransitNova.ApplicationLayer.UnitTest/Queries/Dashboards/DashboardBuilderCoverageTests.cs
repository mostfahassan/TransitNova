using FluentAssertions;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Features.Carriers;
using TransitNova.BusinessLayer.Features.UserOperations;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.ApplicationLayer.Tests.Queries.Dashboards;

public sealed class DashboardBuilderCoverageTests
{
    [Fact]
    public void CarrierDashboardBuilder_Should_AggregateStatusesAndOrderActiveTrips()
    {
        var carrierId = Guid.NewGuid();
        var oldTrip = new CarrierTripDto { Id = Guid.NewGuid(), Status = TripStatus.Active, PlannedDate = DateTime.UtcNow.AddDays(-2) };
        var nextTrip = new CarrierTripDto { Id = Guid.NewGuid(), Status = TripStatus.Planned, PlannedDate = DateTime.UtcNow.AddDays(-1) };
        var completedTrip = new CarrierTripDto { Id = Guid.NewGuid(), Status = TripStatus.Completed, PlannedDate = DateTime.UtcNow.AddDays(-3) };
        var shipment = new RetrieveShipmentDto
        {
            Id = Guid.NewGuid(),
            TrackingNumber = "TN-100",
            CurrentStatus = ShipmentStatuses.OutForDelivery,
            CreatedAt = DateTime.UtcNow,
            Receiver = new UserSummaryDto { FullName = "Mona Ali" }
        };
        var statuses = new Dictionary<ShipmentStatuses, int>
        {
            [ShipmentStatuses.AssignedToPickUpCarrier] = 1,
            [ShipmentStatuses.OutForDelivery] = 2,
            [ShipmentStatuses.Delivered] = 5,
            [ShipmentStatuses.Pending] = 3
        };

        var result = CarrierDashboardBuilder.Build(
            statuses,
            [nextTrip, completedTrip, oldTrip],
            1250m,
            carrierId,
            "Ahmed Carrier",
            [shipment]);

        result.Id.Should().Be(carrierId);
        result.AssignedShipmentsCount.Should().Be(3);
        result.DeliveredShipmentsCount.Should().Be(5);
        result.PendingShipmentsCount.Should().Be(3);
        result.ActiveTrips.Should().Equal(oldTrip, nextTrip);
        result.ActiveTripsCount.Should().Be(2);
        result.RevenueSummary.Should().Be(1250m);
        result.RecentActivity.Should().ContainSingle(activity =>
            activity.Title == "TN-100" && activity.Description.Contains("Mona Ali", StringComparison.Ordinal));
    }

    [Fact]
    public void ProfileDashboardHelper_Should_AggregateActiveAndIssueStatuses()
    {
        var shipments = new[]
        {
            new RetrieveShipmentSummaryDto { Id = Guid.NewGuid(), CurrentStatus = ShipmentStatuses.Pending },
            new RetrieveShipmentSummaryDto { Id = Guid.NewGuid(), CurrentStatus = ShipmentStatuses.Delivered }
        };
        var statuses = new Dictionary<ShipmentStatuses, int>
        {
            [ShipmentStatuses.Pending] = 1,
            [ShipmentStatuses.Delivered] = 1,
            [ShipmentStatuses.InTransit] = 2,
            [ShipmentStatuses.OutForPickup] = 1,
            [ShipmentStatuses.OutForDelivery] = 3,
            [ShipmentStatuses.Issue] = 1,
            [ShipmentStatuses.Cancelled] = 2,
            [ShipmentStatuses.Rejected] = 1
        };

        var result = ProfileDashboardHelper.Build(shipments, statuses);

        result.TotalShipments.Should().Be(2);
        result.PendingShipments.Should().Be(1);
        result.DeliveredShipments.Should().Be(1);
        result.ActiveShipments.Should().Be(6);
        result.IssueShipments.Should().Be(4);
        result.ShipmentSummary.Should().Equal(shipments);
        ProfileDashboardHelper.Empty().TotalShipments.Should().Be(0);
    }

    [Fact]
    public void CarrierProfileBuilder_Should_MapCarrierIdentityAndAdditionalInfo()
    {
        var appUserId = Guid.NewGuid();
        var carrier = Carrier.Create(
            appUserId,
            "Ahmed",
            "Ali",
            "carrier@example.com",
            "+201001234567",
            Address.Create("Cairo", null, "Main Street"),
            1);
        carrier.AddAdditionalData(
            appUserId,
            "LIC-100",
            12,
            15m,
            6,
            DateTime.UtcNow.AddDays(1),
            2,
            Guid.NewGuid());

        var result = CarrierProfileBuilder.FromCarrier(carrier);

        result.Id.Should().Be(carrier.Id);
        result.FullName.Should().Be("Ahmed Ali");
        result.LicenseNumber.Should().Be("LIC-100");
        result.Experience.Should().Be(6);
        result.DefaultCostPerKg.Should().Be(15m);
        result.Address.MainAddress.Should().Be("Cairo");
        result.Vehicle.Should().BeNull();
    }
}
