using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.BusinessLayer.Services.BundleService;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class BundleBenefitServiceTests
{
    [Fact]
    public async Task CalculateShipmentBenefitAsync_NoActiveSubscription_Should_ReturnOriginalCostWithoutBenefitAsync()
    {
        var fixture = new Fixture();
        fixture.Repository
            .Setup(x => x.GetActiveSubscriptionForUserAsync(fixture.UserProfileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ActiveBundleSubscriptionBenefitDto?)null);

        var result = await fixture.Service.CalculateShipmentBenefitAsync(
            fixture.UserProfileId,
            250m,
            Package(weight: 5m),
            CancellationToken.None);

        result.SubscriptionBenefitApplied.Should().BeFalse();
        result.OriginalShippingCost.Should().Be(250m);
        result.FinalShippingCost.Should().Be(250m);
        result.DiscountAmount.Should().Be(0m);
        fixture.Repository.Verify(
            x => x.GetMonthlyAppliedBenefitCountAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CalculateShipmentBenefitAsync_EligibleShipment_Should_ApplyDiscountAndPersistAuditValuesInResultAsync()
    {
        var fixture = new Fixture();
        var subscription = fixture.ActiveSubscription(discountPercentage: 12.5m, minimumValue: 100m, maxMonthlyShipments: 3);
        fixture.ArrangeActiveSubscription(subscription, appliedThisMonth: 1);

        var result = await fixture.Service.CalculateShipmentBenefitAsync(
            fixture.UserProfileId,
            480m,
            Package(weight: 10m),
            CancellationToken.None);

        result.SubscriptionBenefitApplied.Should().BeTrue();
        result.BundleSubscriptionId.Should().Be(subscription.SubscriptionId);
        result.BundleId.Should().Be(subscription.BundleId);
        result.BundleName.Should().Be(subscription.BundleName);
        result.OriginalShippingCost.Should().Be(480m);
        result.DiscountPercentage.Should().Be(12.5m);
        result.DiscountAmount.Should().Be(60m);
        result.FinalShippingCost.Should().Be(420m);
        result.SubscriptionBenefitMessage.Should().Contain(subscription.BundleName);
        fixture.Repository.VerifyMonthlyQuotaChecked(fixture.UserProfileId, subscription.BundleId);
    }

    [Fact]
    public async Task CalculateShipmentBenefitAsync_WeightExceedsBundleLimit_Should_NotCheckMonthlyQuotaAsync()
    {
        var fixture = new Fixture();
        var subscription = fixture.ActiveSubscription(maxWeight: 15m);
        fixture.ArrangeActiveSubscription(subscription, appliedThisMonth: 0);

        var result = await fixture.Service.CalculateShipmentBenefitAsync(
            fixture.UserProfileId,
            300m,
            Package(weight: 20m),
            CancellationToken.None);

        result.SubscriptionBenefitApplied.Should().BeFalse();
        result.FinalShippingCost.Should().Be(300m);
        result.SubscriptionBenefitMessage.Should().Be("Shipment weight is outside the active bundle limits.");
        fixture.Repository.Verify(
            x => x.GetMonthlyAppliedBenefitCountAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CalculateShipmentBenefitAsync_CostBelowMinimumThreshold_Should_NotApplyDiscountAsync()
    {
        var fixture = new Fixture();
        var subscription = fixture.ActiveSubscription(minimumValue: 500m);
        fixture.ArrangeActiveSubscription(subscription, appliedThisMonth: 0);

        var result = await fixture.Service.CalculateShipmentBenefitAsync(
            fixture.UserProfileId,
            499.99m,
            Package(weight: 5m),
            CancellationToken.None);

        result.SubscriptionBenefitApplied.Should().BeFalse();
        result.FinalShippingCost.Should().Be(499.99m);
        result.SubscriptionBenefitMessage.Should().Be("Shipment value is below the active bundle discount threshold.");
        fixture.Repository.Verify(
            x => x.GetMonthlyAppliedBenefitCountAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CalculateShipmentBenefitAsync_MonthlyBenefitQuotaReached_Should_NotApplyDiscountAsync()
    {
        var fixture = new Fixture();
        var subscription = fixture.ActiveSubscription(maxMonthlyShipments: 2);
        fixture.ArrangeActiveSubscription(subscription, appliedThisMonth: 2);

        var result = await fixture.Service.CalculateShipmentBenefitAsync(
            fixture.UserProfileId,
            700m,
            Package(weight: 4m),
            CancellationToken.None);

        result.SubscriptionBenefitApplied.Should().BeFalse();
        result.FinalShippingCost.Should().Be(700m);
        result.SubscriptionBenefitMessage.Should().Be("Monthly bundle shipment benefits have already been used.");
        fixture.Repository.VerifyMonthlyQuotaChecked(fixture.UserProfileId, subscription.BundleId);
    }

    [Fact]
    public async Task CalculateShipmentBenefitAsync_ZeroCost_Should_NotCallRepositoryAsync()
    {
        var fixture = new Fixture();

        var result = await fixture.Service.CalculateShipmentBenefitAsync(
            fixture.UserProfileId,
            0m,
            Package(weight: 1m),
            CancellationToken.None);

        result.SubscriptionBenefitApplied.Should().BeFalse();
        result.FinalShippingCost.Should().Be(0m);
        result.SubscriptionBenefitMessage.Should().Be("Shipment cost is not eligible for a subscription benefit.");
        fixture.Repository.VerifyNoOtherCalls();
    }

    private static PackageSpecificationDto Package(decimal weight) => new()
    {
        Weight = weight,
        Width = 10m,
        Height = 10m,
        Length = 10m
    };

    private sealed class Fixture
    {
        public Guid UserProfileId { get; } = Guid.NewGuid();
        public Mock<IBundleSubscriptionQueryRepository> Repository { get; } = new();
        public BundleBenefitService Service { get; }

        public Fixture()
        {
            Service = new BundleBenefitService(
                Repository.Object,
                Mock.Of<ILogger<BundleBenefitService>>());
        }

        public ActiveBundleSubscriptionBenefitDto ActiveSubscription(
            decimal discountPercentage = 10m,
            decimal minimumValue = 100m,
            int maxMonthlyShipments = 5,
            decimal maxWeight = 30m) => new()
        {
            SubscriptionId = Guid.NewGuid(),
            BundleId = Guid.NewGuid(),
            BundleName = "Pro Saver",
            SubscriptionDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddMonths(1),
            MaxShipmentsPerMonth = maxMonthlyShipments,
            MaxWeightPerShipment = maxWeight,
            MaxDistancePerShipment = 100m,
            DiscountPercentage = discountPercentage,
            MinimumShipmentValueForDiscount = minimumValue
        };

        public void ArrangeActiveSubscription(ActiveBundleSubscriptionBenefitDto subscription, int appliedThisMonth)
        {
            Repository
                .Setup(x => x.GetActiveSubscriptionForUserAsync(UserProfileId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(subscription);
            Repository
                .Setup(x => x.GetMonthlyAppliedBenefitCountAsync(
                    UserProfileId,
                    subscription.BundleId,
                    It.Is<DateTime>(d => d.Kind == DateTimeKind.Utc && d.Day == 1),
                    It.Is<DateTime>(d => d.Kind == DateTimeKind.Utc && d.Day == 1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(appliedThisMonth);
        }
    }
}

internal static class BundleBenefitRepositoryMockExtensions
{
    public static void VerifyMonthlyQuotaChecked(
        this Mock<IBundleSubscriptionQueryRepository> repository,
        Guid userProfileId,
        Guid bundleId)
    {
        repository.Verify(
            x => x.GetMonthlyAppliedBenefitCountAsync(
                userProfileId,
                bundleId,
                It.Is<DateTime>(d => d.Kind == DateTimeKind.Utc && d.Day == 1),
                It.Is<DateTime>(d => d.Kind == DateTimeKind.Utc && d.Day == 1),
                CancellationToken.None),
            Times.Once);
    }
}