using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Bundles;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Bundle;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Commands.Bundles;

public sealed class BundleSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task SubscribeToBundleHandler_ServiceSuccess_Should_ReturnBundleInvoiceAsync()
    {
        var fixture = new SubscriptionFixture();
        var command = fixture.CreateSubscribeCommand();
        var invoice = fixture.CreateBundleInvoice();

        fixture.BundleSubscription
            .Setup(x => x.HandleBundleSubscription(command.UserId, command.BundleId, command.Dto.PaymentMethod, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<BundlePaymentInvoiceDto>.Success(invoice));

        var result = await fixture.CreateSubscribeHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(invoice);
        fixture.BundleSubscription.Verify(
            x => x.HandleBundleSubscription(command.UserId, command.BundleId, PaymentMethod.CreditCard, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task SubscribeToBundleHandler_ServiceFailure_Should_ReturnFailureAsync()
    {
        var fixture = new SubscriptionFixture();
        var command = fixture.CreateSubscribeCommand();

        fixture.BundleSubscription
            .Setup(x => x.HandleBundleSubscription(command.UserId, command.BundleId, command.Dto.PaymentMethod, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<BundlePaymentInvoiceDto>.Failure(Errors.FailedOperation("Payment failed")));

        var result = await fixture.CreateSubscribeHandler().Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.BundleSubscription.Verify(
            x => x.HandleBundleSubscription(command.UserId, command.BundleId, PaymentMethod.CreditCard, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task UnsubscribeFromBundleHandler_MissingUser_Should_ReturnNotFoundWithoutLoadingBundleAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.Users.Setup(x => x.GetAppUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        var result = await fixture.CreateUnsubscribeHandler().Handle(
            new UnsubscribeFromBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.Bundles.Verify(
            x => x.GetByIdAsync<Bundle>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UnsubscribeFromBundleHandler_MissingBundle_Should_ReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.ArrangeExistingUser();
        fixture.Bundles.Setup(x => x.GetByIdAsync<Bundle>(fixture.Bundle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bundle?)null);

        var result = await fixture.CreateUnsubscribeHandler().Handle(
            new UnsubscribeFromBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UnsubscribeFromBundleHandler_NoActiveSubscription_Should_ReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.ArrangeExistingUserAndBundle();

        var result = await fixture.CreateUnsubscribeHandler().Handle(
            new UnsubscribeFromBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UnsubscribeFromBundleHandler_ActiveSubscription_Should_DeactivateSaveAndInvalidateCachesAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.Bundle.Subscribe(fixture.UserId);
        fixture.ArrangeExistingUserAndBundle();

        var result = await fixture.CreateUnsubscribeHandler().Handle(
            new UnsubscribeFromBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fixture.Bundle.Subscriptions.Should().ContainSingle(x =>
            x.SubscribedUserId == fixture.UserId && !x.IsActive && x.CancelledAt.HasValue);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UnsubscribeFromBundleHandler_AlreadyInactiveSubscription_Should_ReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.Bundle.Subscribe(fixture.UserId);
        fixture.Bundle.Unsubscribe(fixture.UserId);
        fixture.ArrangeExistingUserAndBundle();

        var result = await fixture.CreateUnsubscribeHandler().Handle(
            new UnsubscribeFromBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private sealed class SubscriptionFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Bundle Bundle { get; } = Bundle.Create("operation-manager", "Business", "Business bundle", 500m, BundleTier.Pro, 1, 20, 200m, 1_000m, 0m, 0m);
        public Mock<IGenericRepository<Bundle, Guid>> Bundles { get; } = new();
        public Mock<IUserQueryRepository> Users { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IBundleSubscription> BundleSubscription { get; } = new();

        public SubscribeToBundleCommand CreateSubscribeCommand() => new(
            Guid.NewGuid(),
            UserId,
            Bundle.Id,
            new SubscribeToBundleDto { PaymentMethod = PaymentMethod.CreditCard });

        public BundlePaymentInvoiceDto CreateBundleInvoice() => new()
        {
            InvoiceId = "INV-TEST",
            PaymentId = Guid.NewGuid(),
            ReferenceId = Bundle.Id,
            BundleId = Bundle.Id,
            BundleName = Bundle.BundleName,
            FullName = "Mostafa Customer",
            BundlePrice = Bundle.BundlePrice,
            Commission = 10m,
            TotalAmount = 510m,
            PaymentMethod = PaymentMethod.CreditCard.ToString(),
            Status = PaymentStatus.Success.ToString(),
            Currency = Currency.EGP,
            PaidAt = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            SubscribedAt = DateTime.UtcNow,
            Notes = "Payment processed successfully."
        };

        public void ArrangeExistingUser() =>
            Users.Setup(x => x.GetAppUserIdAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(UserId);

        public void ArrangeExistingUserAndBundle()
        {
            ArrangeExistingUser();
            Bundles.Setup(x => x.GetByIdAsync<Bundle>(Bundle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Bundle);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        public SubscribeToBundleHandler CreateSubscribeHandler() => new(
            BundleSubscription.Object,
            Mock.Of<ILogger<SubscribeToBundleHandler>>());

        public UnsubscribeFromBundleHandler CreateUnsubscribeHandler() => new(
            Bundles.Object,
            Users.Object,
            UnitOfWork.Object,
            Mock.Of<ILogger<UnsubscribeFromBundleHandler>>());
    }
}

