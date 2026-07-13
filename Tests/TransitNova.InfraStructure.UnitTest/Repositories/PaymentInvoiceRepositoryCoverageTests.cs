using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransitNova.Domain.Contracts.Constants;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Bundle;
using TransitNova.Domain.Enums.Payment;
using TransitNova.InfraStructure.Repository.PaymmentInvoice;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class PaymentInvoiceRepositoryCoverageTests
{
    [Fact]
    public async Task ShipmentInvoiceQueries_Should_ProjectBenefitAndOwnershipFieldsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture, "Invoice");
        var (_, user) = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "Invoice");
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user, location.City);
        var paymentId = Guid.NewGuid();
        var bundleId = Guid.NewGuid();
        var invoice = PaymentInvoice.Create(
            paymentId,
            shipment.Id,
            user.Id,
            90m,
            5m,
            95m,
            PaymentMethod.CreditCard,
            PaymentStatus.Success,
            Constant.PaymentReferenceConstants.Shipment,
            DateTime.UtcNow,
            "Benefit applied",
            Guid.NewGuid(),
            bundleId,
            "Starter",
            100m,
            10m,
            10m,
            90m,
            true);
        fixture.Context.PaymentInvoices.Add(invoice);
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();
        var sut = new PaymentRepositoryQuery(fixture.Context);

        var byPayment = await sut.GetInvoiceByPaymentIdAsync(paymentId, CancellationToken.None);
        var byUser = await sut.GetUserInvoiceAsync(user.Id, CancellationToken.None);
        var byBoth = await sut.GetUserInvoiceByPaymentIdAsync(user.Id, paymentId, CancellationToken.None);
        var allForUser = await sut.GetUserInvoicesAsync(user.Id, CancellationToken.None);

        byPayment.Should().NotBeNull();
        byPayment!.ShipmentId.Should().Be(shipment.Id);
        byPayment.ShipmentTrackingNumber.Should().Be(shipment.TrackingNumber);
        byPayment.CustomerName.Should().Be(user.FullName);
        byPayment.OriginalShippingCost.Should().Be(100m);
        byPayment.DiscountAmount.Should().Be(10m);
        byPayment.FinalShippingCost.Should().Be(90m);
        byPayment.SubscriptionBenefitApplied.Should().BeTrue();
        byPayment.SubscriptionBenefitMessage.Should().Contain("applied");
        byUser!.PaymentId.Should().Be(paymentId);
        byBoth!.PaymentId.Should().Be(paymentId);
        allForUser.Should().ContainSingle(item => item.PaymentId == paymentId);
        (await sut.GetUserInvoiceByPaymentIdAsync(Guid.NewGuid(), paymentId, CancellationToken.None)).Should().BeNull();
    }

    [Fact]
    public async Task UserInvoiceQueries_Should_AcceptAuthenticatedAppUserIdAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture, "AppUserInvoice");
        var (appUser, profile) = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "AppUserInvoice");
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, profile, location.City);
        var paymentId = Guid.NewGuid();
        fixture.Context.PaymentInvoices.Add(PaymentInvoice.Create(
            paymentId,
            shipment.Id,
            profile.Id,
            100m,
            5m,
            105m,
            PaymentMethod.CreditCard,
            PaymentStatus.Success,
            Constant.PaymentReferenceConstants.Shipment));
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var sut = new PaymentRepositoryQuery(fixture.Context);

        var byPayment = await sut.GetUserInvoiceByPaymentIdAsync(appUser.Id, paymentId, CancellationToken.None);
        var allForUser = await sut.GetUserInvoicesAsync(appUser.Id, CancellationToken.None);

        byPayment.Should().NotBeNull();
        byPayment!.PaymentId.Should().Be(paymentId);
        allForUser.Should().ContainSingle(item => item.PaymentId == paymentId);
    }
    [Fact]
    public async Task GetInvoicesPagedAsync_Should_ClampPagingAndReturnNewestFirstAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture, "Paging");
        var (_, user) = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "Paging");
        var firstShipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user, location.City);
        var secondShipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user, location.City);
        var first = PaymentInvoice.Create(Guid.NewGuid(), firstShipment.Id, user.Id, 100m, 5m, 105m,
            PaymentMethod.CreditCard, PaymentStatus.Success, Constant.PaymentReferenceConstants.Shipment);
        var second = PaymentInvoice.Create(Guid.NewGuid(), secondShipment.Id, user.Id, 200m, 10m, 210m,
            PaymentMethod.PayPal, PaymentStatus.Success, Constant.PaymentReferenceConstants.Shipment);
        typeof(PaymentInvoice).BaseType!.GetProperty("CreatedAt")!.SetValue(first, DateTime.UtcNow.AddMinutes(-2));
        typeof(PaymentInvoice).BaseType!.GetProperty("CreatedAt")!.SetValue(second, DateTime.UtcNow.AddMinutes(-1));
        fixture.Context.PaymentInvoices.AddRange(first, second);
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();
        var sut = new PaymentRepositoryQuery(fixture.Context);

        var result = await sut.GetInvoicesPagedAsync(0, 500, CancellationToken.None);

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(100);
        result.TotalCount.Should().Be(2);
        result.Data.Select(item => item.PaymentId).Should().Equal(second.PaymentId, first.PaymentId);
    }

    [Fact]
    public async Task GetBundleInvoiceByPaymentIdAsync_Should_ProjectLatestSubscriptionAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture, "BundleInvoice");
        var (_, user) = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "BundleInvoice");
        var bundle = Bundle.Create("admin", "Pro", "Pro bundle", 500m, BundleTier.Pro, 1, 10, 100m, 500m, 10m, 100m);
        fixture.Context.Bundles.Add(bundle);
        await fixture.Context.SaveChangesAsync();
        var subscription = new BundleSubscription
        {
            BundleId = bundle.Id,
            Bundle = bundle,
            SubscribedUserId = user.Id,
            IsActive = true,
            SubscriptionDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddMonths(1)
        };
        fixture.Context.UserBundleSubscriptions.Add(subscription);
        var paymentId = Guid.NewGuid();
        fixture.Context.PaymentInvoices.Add(PaymentInvoice.Create(
            paymentId,
            bundle.Id,
            user.Id,
            bundle.BundlePrice,
            25m,
            525m,
            PaymentMethod.PayPal,
            PaymentStatus.Success,
            Constant.PaymentReferenceConstants.Bundle,
            DateTime.UtcNow,
            "Subscribed"));
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var result = await new PaymentRepositoryQuery(fixture.Context)
            .GetBundleInvoiceByPaymentIdAsync(paymentId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.BundleId.Should().Be(bundle.Id);
        result.BundleName.Should().Be("Pro");
        result.FullName.Should().Be(user.FullName);
        result.SubscribedAt.Should().BeCloseTo(subscription.SubscriptionDate, TimeSpan.FromSeconds(1));
        result.EndDate.Should().BeCloseTo(subscription.EndDate!.Value, TimeSpan.FromSeconds(1));
    }
}


