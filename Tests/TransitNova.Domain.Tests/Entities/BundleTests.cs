using FluentAssertions;
using System;
using System.Linq;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums;
using TransitNova.Domain.Enums.Bundle;

namespace TransitNova.Domain.Tests.Entities;

public sealed class BundleTests
{
    [Fact]
    public void CreateBundle_Should_Create_ActiveBundle_When_DataIsValid()
    {
        var bundle = Bundle.Create("admin", "Starter", "Basic", 100m, BundleTier.Standard, 1, 10, 50m, 100m, 10m, 2000m);

        bundle.BundleName.Should().Be("Starter");
        bundle.BundleDescription.Should().Be("Basic");
        bundle.BundlePrice.Should().Be(100m);
        bundle.Tier.Should().Be(BundleTier.Standard);
        bundle.BundleDurationMonths.Should().Be(1);
        bundle.MaxShipmentsPerMonth.Should().Be(10);
        bundle.MaxWeightPerShipment.Should().Be(50m);
        bundle.MaxDistancePerShipment.Should().Be(100m);
        bundle.DiscountPercentage.Should().Be(10m);
        bundle.MinimumShipmentValueForDiscount.Should().Be(2000m);
        bundle.CreatedBy.Should().Be("admin");
        bundle.CurrentState.Should().BeTrue();
    }

    [Fact]
    public void UpdateBundle_Should_UpdateValuesAndRaiseEvent_When_Called()
    {
        var bundle = CreateBundle();

        bundle.Update("admin-2", 200m, "Updated Description", 15m, 2500m);

        bundle.BundlePrice.Should().Be(200m);
        bundle.BundleDescription.Should().Be("Updated Description");
        bundle.DiscountPercentage.Should().Be(15m);
        bundle.MinimumShipmentValueForDiscount.Should().Be(2500m);
        bundle.UpdatedBy.Should().Be("admin-2");
        bundle.GetDomainEvents().Should().ContainSingle(e => e is BundleUpdatedDomainEvent);
    }

    [Fact]
    public void Subscribe_Should_CreateActiveSubscriptionAndRaiseEvent_When_UserIsNotSubscribed()
    {
        var bundle = CreateBundle();
        var userId = Guid.NewGuid();

        bundle.Subscribe(userId);

        var subscription = bundle.Subscriptions.Should().ContainSingle().Subject;
        subscription.SubscribedUserId.Should().Be(userId);
        subscription.IsActive.Should().BeTrue();
        subscription.EndDate.Should().BeCloseTo(subscription.SubscriptionDate.AddMonths(bundle.BundleDurationMonths), TimeSpan.FromSeconds(1));
        bundle.GetDomainEvents().Should().Contain(e => e is UserSubscribedToBundleDomainEvent);
    }

    [Fact]
    public void Subscribe_Should_ThrowException_When_UserHasActiveSubscription()
    {
        var bundle = CreateBundle();
        var userId = Guid.NewGuid();
        bundle.Subscribe(userId);

        var act = () => bundle.Subscribe(userId);

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("BUNDLE_ALREADY_SUBSCRIBED");
    }

    [Fact]
    public void Unsubscribe_Should_DeactivateSubscriptionAndRaiseEvent_When_ActiveSubscriptionExists()
    {
        var bundle = CreateBundle();
        var userId = Guid.NewGuid();
        bundle.Subscribe(userId);

        bundle.Unsubscribe(userId);

        var subscription = bundle.Subscriptions.Single();
        subscription.IsActive.Should().BeFalse();
        subscription.CancelledAt.Should().NotBeNull();
        bundle.GetDomainEvents().Should().Contain(e => e is UserUnsubscribedFromBundleDomainEvent);
    }

    [Fact]
    public void Unsubscribe_Should_ThrowException_When_ActiveSubscriptionDoesNotExist()
    {
        var bundle = CreateBundle();

        var act = () => bundle.Unsubscribe(Guid.NewGuid());

        act.Should().Throw<EntityNotFoundException>().Which.ErrorCode.Should().Be("BUNDLE_SUBSCRIPTION_NOT_FOUND");
    }

    [Fact]
    public void Subscribe_Should_AllowResubscription_When_PreviousSubscriptionIsInactive()
    {
        var bundle = CreateBundle();
        var userId = Guid.NewGuid();
        bundle.Subscribe(userId);
        bundle.Unsubscribe(userId);

        bundle.Subscribe(userId);

        bundle.Subscriptions.Should().HaveCount(2);
        bundle.Subscriptions.Count(s => s.IsActive).Should().Be(1);
    }

    private static Bundle CreateBundle() =>
        Bundle.Create("admin", "Starter", "Basic", 100m, BundleTier.Standard, 1, 10, 50m, 100m, 10m, 2000m);
}