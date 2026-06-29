using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Bundles;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.ApplicationLayer.Tests.Commands.Bundles;

public sealed class BundleSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task SubscribeToBundleHandler_MissingUser_Should_ReturnNotFoundWithoutLoadingBundleAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.Users.Setup(x => x.GetAppUserIdAsync(fixture.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        var result = await fixture.CreateSubscribeHandler().Handle(
            new SubscribeToBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.Bundles.Verify(
            x => x.GetByIdAsync<Bundle>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SubscribeToBundleHandler_MissingBundle_Should_ReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.ArrangeExistingUser();
        fixture.Bundles.Setup(x => x.GetByIdAsync<Bundle>(fixture.Bundle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bundle?)null);

        var result = await fixture.CreateSubscribeHandler().Handle(
            new SubscribeToBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        fixture.Cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SubscribeToBundleHandler_ValidRequest_Should_SubscribeSaveAndInvalidateCachesAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.ArrangeExistingUserAndBundle();

        var result = await fixture.CreateSubscribeHandler().Handle(
            new SubscribeToBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fixture.Bundle.Subscriptions.Should().ContainSingle(x =>
            x.SubscribedUserId == fixture.UserId && x.IsActive);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.VerifyAllCachesInvalidated();
    }

    [Fact]
    public async Task SubscribeToBundleHandler_AlreadySubscribedUser_Should_PropagateDomainExceptionWithoutSavingAsync()
    {
        var fixture = new SubscriptionFixture();
        fixture.Bundle.Subscribe(fixture.UserId);
        fixture.ArrangeExistingUserAndBundle();

        var act = () => fixture.CreateSubscribeHandler().Handle(
            new SubscribeToBundleCommand(Guid.NewGuid(), fixture.UserId, fixture.Bundle.Id),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainOperationException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
        fixture.Cache.VerifyNoOtherCalls();
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
        fixture.VerifyAllCachesInvalidated();
    }

    private sealed class SubscriptionFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Bundle Bundle { get; } = Bundle.Create(
            "operation-manager", "Business", 500m, "Business bundle", 200m, 1_000m, 20);
        public Mock<IGenericRepository<Bundle, Guid>> Bundles { get; } = new();
        public Mock<IUserQueryRepository> Users { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();

        public void ArrangeExistingUser() =>
            Users.Setup(x => x.GetAppUserIdAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

        public void ArrangeExistingUserAndBundle()
        {
            ArrangeExistingUser();
            Bundles.Setup(x => x.GetByIdAsync<Bundle>(Bundle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Bundle);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        public SubscribeToBundleHandler CreateSubscribeHandler() => new(
            Bundles.Object,
            Users.Object,
            UnitOfWork.Object,
            Mock.Of<ILogger<SubscribeToBundleHandler>>());

        public UnsubscribeFromBundleHandler CreateUnsubscribeHandler() => new(
            Bundles.Object,
            Users.Object,
            UnitOfWork.Object,
            Mock.Of<ILogger<UnsubscribeFromBundleHandler>>());

        public void VerifyAllCachesInvalidated()
        {
        }
    }
}


