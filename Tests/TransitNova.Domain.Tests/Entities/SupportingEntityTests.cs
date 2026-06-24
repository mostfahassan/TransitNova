using FluentAssertions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.Domain.Tests.Entities;

public sealed class SupportingEntityTests
{
    [Fact]
    public void CreateCarrierRating_Should_PreserveRatingDetails_When_DataIsValid()
    {
        var carrierId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var rating = CarrierRating.Create(carrierId, shipmentId, customerId, 5, "Excellent");

        rating.Id.Should().NotBeEmpty();
        rating.CarrierId.Should().Be(carrierId);
        rating.ShipmentId.Should().Be(shipmentId);
        rating.CustomerId.Should().Be(customerId);
        rating.Rating.Should().Be(5);
        rating.Comment.Should().Be("Excellent");
        rating.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CreatePayment_Should_PreservePaymentDetails_When_DataIsValid()
    {
        var shipmentId = Guid.NewGuid();

    }

    [Fact]
    public void AddLog_Should_CreateActivityLog_When_AllActorDetailsAreAvailable()
    {
        var actorId = Guid.NewGuid();

        var log = SystemActivityLog.AddLog(
            ActivityAction.Created,
            ActivityEntityType.Shipment,
            "Shipment created",
            actorId,
            "Ahmed Ali");

        log.Action.Should().Be(ActivityAction.Created);
        log.EntityType.Should().Be(ActivityEntityType.Shipment);
        log.Description.Should().Be("Shipment created");
        log.PerformedByUserId.Should().Be(actorId);
        log.PerformedByName.Should().Be("Ahmed Ali");
        log.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void AddLog_Should_AllowMissingActorId_When_ActorIsExternal()
    {
        var log = SystemActivityLog.AddLog(
            ActivityAction.Updated,
            ActivityEntityType.Warehouse,
            "Warehouse updated",
            null,
            "System");

        log.PerformedByUserId.Should().BeNull();
        log.PerformedByName.Should().Be("System");
    }

    [Fact]
    public void BundleSubscription_Should_GenerateUniqueIdentifier_When_Constructed()
    {
        var first = new BundleSubscription();
        var second = new BundleSubscription();

        first.Id.Should().NotBeEmpty();
        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void IdempotentTable_Should_PreserveRequestMetadata_When_Constructed()
    {
        var requestId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var entry = new IdempotentTable
        {
            RequestId = requestId,
            InstanceName = "CreateShipmentCommand",
            CreatedAt = createdAt
        };

        entry.RequestId.Should().Be(requestId);
        entry.InstanceName.Should().Be("CreateShipmentCommand");
        entry.CreatedAt.Should().Be(createdAt);
    }
}
