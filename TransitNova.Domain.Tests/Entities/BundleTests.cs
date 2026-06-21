using FluentAssertions;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.Domain.Tests.Entities;

public sealed class BundleTests
{
    [Fact]
    public void CreateBundle_Should_Create_ActiveBundle_When_DataIsValid()
    {
        var bundle = Bundle.Create("admin", "Starter", 100m, "Basic", 50m, 100m, 10);

        bundle.BundleName.Should().Be("Starter");
        bundle.BundlePrice.Should().Be(100m);
        bundle.TotalWeight.Should().Be(50m);
        bundle.TotalDistance.Should().Be(100m);
        bundle.TotalShipments.Should().Be(10);
        bundle.CreatedBy.Should().Be("admin");
        bundle.CurrentState.Should().BeTrue();
    }

    [Fact]
    public void UpdateBundle_Should_UpdateValuesAndRaiseEvent_When_Called()
    {
        var bundle = CreateBundle();

        bundle.Update("admin-2", 200m, 80m, 20);

        bundle.BundlePrice.Should().Be(200m);
        bundle.TotalWeight.Should().Be(80m);
        bundle.TotalShipments.Should().Be(20);
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
        subscription.EndDate.Should().BeCloseTo(subscription.SubscriptionDate.AddMonths(1), TimeSpan.FromSeconds(1));
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
        bundle.GetDomainEvents().Should().Contain(e => e is UserUnSubscribedToBundleDomainEvent);
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
        Bundle.Create("admin", "Starter", 100m, "Basic", 50m, 100m, 10);
}
