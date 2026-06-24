using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class CancelIssueWorkflowPhase2Tests
{
    [Fact]
    public async Task CancelShipmentHandler_WhenShipmentExists_ShouldTransitionToCancelledAsync()
    {
        var fixture = new CancellationFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fixture.Shipment.CurrentStatus.Should().Be(ShipmentStatuses.Cancelled);
    }

    [Fact]
    public async Task CancelShipmentHandler_WhenShipmentExists_ShouldSetCancellationTimestampAsync()
    {
        var fixture = new CancellationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Shipment.IsCancelled.Should().BeTrue();
        fixture.Shipment.CancelledOn.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelShipmentHandler_WhenShipmentExists_ShouldCreateCancelledActivityLogAsync()
    {
        var fixture = new CancellationFixture();
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        captured!.Action.Should().Be(ActivityAction.Cancelled);
        captured.PerformedByName.Should().Be("Mona Ali");
    }

    [Fact]
    public async Task CancelShipmentHandler_WhenShipmentExists_ShouldSaveAndInvalidateSevenCachesAsync()
    {
        var fixture = new CancellationFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(7));
    }

    [Fact]
    public async Task CancelShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new CancellationFixture();
        fixture.Shipments.Setup(x => x.GetShipmentForCommandsAsync(
                fixture.Shipment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelShipmentHandler_WhenShipmentIsDelivered_ShouldPropagateInvalidTransitionWithoutSavingAsync()
    {
        var fixture = new CancellationFixture(ShipmentTestData.CreateDeliveredShipment());

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidShipmentStateException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelShipmentHandler_WhenSaveReportsNegativeResult_ShouldThrowDomainOperationExceptionAsync()
    {
        var fixture = new CancellationFixture();
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(-1);

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainOperationException>()
            .Where(x => x.ErrorCode == "SHIPMENT_CANCELLATION_FAILED");
        fixture.Cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenDeliveredShipmentExists_ShouldTransitionToIssueAsync()
    {
        var fixture = new IssueFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fixture.Shipment.CurrentStatus.Should().Be(ShipmentStatuses.Issue);
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenDeliveredShipmentExists_ShouldPersistIssueMessageAndTimestampAsync()
    {
        var fixture = new IssueFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Shipment.IssueMessage.Should().Be(fixture.Command.IssueMessage);
        fixture.Shipment.IssuedOn.Should().NotBeNull();
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenDeliveredShipmentExists_ShouldSaveAndInvalidateSevenCachesAsync()
    {
        var fixture = new IssueFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(7));
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnFailureWithoutSavingAsync()
    {
        var fixture = new IssueFixture();
        fixture.Shipments.Setup(x => x.GetShipmentForCommandsAsync(
                fixture.Shipment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(ShipmentStatuses.Pending)]
    [InlineData(ShipmentStatuses.Approved)]
    [InlineData(ShipmentStatuses.Cancelled)]
    public async Task IssueShipmentHandler_WhenShipmentIsNotDelivered_ShouldPropagateInvalidStateWithoutSavingAsync(
        ShipmentStatuses status)
    {
        var shipment = ShipmentTestData.CreateShipment();
        if (status == ShipmentStatuses.Approved) shipment.ApproveShipment(Guid.NewGuid());
        if (status == ShipmentStatuses.Cancelled) shipment.CancelShipment();
        var fixture = new IssueFixture(shipment);

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidShipmentStateException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private sealed class CancellationFixture
    {
        public Shipment Shipment { get; }
        public CancelShipmentCommand Command { get; }
        public Mock<IShipmentQueryRepository> Shipments { get; } = new();
        public Mock<IUserAuthQueryService> Users { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public CancelShipmentHandler Handler { get; }

        public CancellationFixture(Shipment? shipment = null)
        {
            Shipment = shipment ?? ShipmentTestData.CreateShipment();
            Command = new CancelShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Shipment.Id);
            Shipments.Setup(x => x.GetShipmentForCommandsAsync(Shipment.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Shipment);
            Users.Setup(x => x.FindByIdAsync(Command.AppUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AppUserDto { Id = Command.AppUserId, FullName = "Mona Ali" });
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new CancelShipmentHandler(
                Shipments.Object, Users.Object, Logs.Object, UnitOfWork.Object, Cache.Object,
                Mock.Of<ILogger<CancelShipmentHandler>>());
        }
    }

    private sealed class IssueFixture
    {
        public Shipment Shipment { get; }
        public IssueShipmentCommand Command { get; }
        public Mock<IShipmentQueryRepository> Shipments { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public IssueShipmentHandler Handler { get; }

        public IssueFixture(Shipment? shipment = null)
        {
            Shipment = shipment ?? ShipmentTestData.CreateDeliveredShipment();
            Command = new IssueShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Shipment.Id, "Package damaged");
            Shipments.Setup(x => x.GetShipmentForCommandsAsync(Shipment.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Shipment);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new IssueShipmentHandler(
                Shipments.Object, UnitOfWork.Object, Cache.Object,
                Mock.Of<ILogger<IssueShipmentHandler>>());
        }
    }
}
