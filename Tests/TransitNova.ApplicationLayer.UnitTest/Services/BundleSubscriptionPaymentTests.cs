using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.BusinessLayer.Services.BundleService;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Bundle;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class BundleSubscriptionPaymentTests
{
    [Fact]
    public async Task HandleBundleSubscription_Should_ReturnNotFound_WhenUserProfileDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.Users.Setup(x => x.GetAppUserIdAsync(fixture.AppUserId, CancellationToken.None)).ReturnsAsync(Guid.Empty);

        var result = await fixture.CreateSut().HandleBundleSubscription(
            fixture.AppUserId, fixture.Bundle.Id, PaymentMethod.CreditCard, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.Bundles.Verify(x => x.GetBundleForSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleBundleSubscription_Should_ReturnNotFound_WhenBundleDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.ArrangeUser();
        fixture.Bundles.Setup(x => x.GetBundleForSubscriptionAsync(fixture.Bundle.Id, CancellationToken.None)).ReturnsAsync((Bundle?)null);

        var result = await fixture.CreateSut().HandleBundleSubscription(
            fixture.AppUserId, fixture.Bundle.Id, PaymentMethod.CreditCard, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.Payment.Verify(x => x.PayForBundle(It.IsAny<CreatePaymentDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleBundleSubscription_Should_ReturnConflict_WhenActiveSubscriptionExistsAsync()
    {
        var fixture = new Fixture();
        fixture.Bundle.Subscribe(fixture.ProfileId);
        fixture.ArrangeUserAndBundle();

        var result = await fixture.CreateSut().HandleBundleSubscription(
            fixture.AppUserId, fixture.Bundle.Id, PaymentMethod.CreditCard, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Contain("already subscribed");
        fixture.Payment.Verify(x => x.PayForBundle(It.IsAny<CreatePaymentDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleBundleSubscription_Should_ReturnPaymentFailure_WithoutSavingAsync()
    {
        var fixture = new Fixture();
        fixture.ArrangeUserAndBundle();
        fixture.Payment.Setup(x => x.PayForBundle(It.IsAny<CreatePaymentDto>(), CancellationToken.None))
            .ReturnsAsync(Result<InvoiceDto>.Failure(Errors.FailedOperation("Payment declined.")));

        var result = await fixture.CreateSut().HandleBundleSubscription(
            fixture.AppUserId, fixture.Bundle.Id, PaymentMethod.CreditCard, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Payment declined.");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleBundleSubscription_Should_SubscribeSaveAndReturnInvoice_WhenPaymentSucceedsAsync()
    {
        var fixture = new Fixture();
        fixture.ArrangeUserAndBundle();
        fixture.Users.Setup(x => x.GetUserFullName(fixture.AppUserId, CancellationToken.None)).ReturnsAsync("Mona Ali");
        CreatePaymentDto? paymentRequest = null;
        fixture.Payment.Setup(x => x.PayForBundle(It.IsAny<CreatePaymentDto>(), CancellationToken.None))
            .Callback<CreatePaymentDto, CancellationToken>((dto, _) => paymentRequest = dto)
            .ReturnsAsync(Result<InvoiceDto>.Success(fixture.PaymentInvoice()));
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

        var result = await fixture.CreateSut().HandleBundleSubscription(
            fixture.AppUserId, fixture.Bundle.Id, PaymentMethod.PayPal, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.BundleId.Should().Be(fixture.Bundle.Id);
        result.Data.FullName.Should().Be("Mona Ali");
        result.Data.InvoiceId.Should().StartWith("INV-");
        paymentRequest!.Cost.Should().Be(fixture.Bundle.BundlePrice);
        paymentRequest.Currency.Should().Be(Currency.EGP);
        fixture.Bundle.Subscriptions.Should().ContainSingle(x => x.SubscribedUserId == fixture.ProfileId && x.IsActive);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    private sealed class Fixture
    {
        internal Guid AppUserId { get; } = Guid.NewGuid();
        internal Guid ProfileId { get; } = Guid.NewGuid();
        internal Bundle Bundle { get; } = Bundle.Create(
            "admin", "Starter", "Starter bundle", 500m, BundleTier.Standard, 1, 10, 25m, 100m, 5m, 100m);
        internal Mock<IBundleSubscriptionCommandRepository> Bundles { get; } = new();
        internal Mock<IUserQueryRepository> Users { get; } = new();
        internal Mock<IPaymentService> Payment { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();

        internal void ArrangeUser() =>
            Users.Setup(x => x.GetAppUserIdAsync(AppUserId, CancellationToken.None)).ReturnsAsync(ProfileId);

        internal void ArrangeUserAndBundle()
        {
            ArrangeUser();
            Bundles.Setup(x => x.GetBundleForSubscriptionAsync(Bundle.Id, CancellationToken.None)).ReturnsAsync(Bundle);
        }

        internal InvoiceDto PaymentInvoice() => new()
        {
            PaymentId = Guid.NewGuid(),
            ReferenceId = Bundle.Id,
            ReferenceType = "Bundle",
            Amount = Bundle.BundlePrice,
            Commission = 10m,
            TotalAmount = Bundle.BundlePrice + 10m,
            PaymentMethod = PaymentMethod.PayPal.ToString(),
            Status = PaymentStatus.Success.ToString(),
            PaidAt = DateTime.UtcNow,
            Notes = "Approved"
        };

        internal BundleSubscriptionPayment CreateSut() => new(
            Bundles.Object,
            Users.Object,
            Payment.Object,
            UnitOfWork.Object,
            NullLogger<BundleSubscriptionPayment>.Instance);
    }
}

