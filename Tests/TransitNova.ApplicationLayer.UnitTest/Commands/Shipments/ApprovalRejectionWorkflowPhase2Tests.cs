using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class ApprovalRejectionWorkflowPhase2Tests
{
    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsPending_ShouldTransitionToApprovedAsync()
    {
        var fixture = new ApprovalFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fixture.Shipment.CurrentStatus.Should().Be(ShipmentStatuses.Approved);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsPending_ShouldRecordOperationManagerIdentityAsync()
    {
        var fixture = new ApprovalFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Shipment.HandledById.Should().Be(fixture.ManagerProfileId);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsPending_ShouldCreateApprovedActivityLogAsync()
    {
        var fixture = new ApprovalFixture();
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        captured!.Action.Should().Be(ActivityAction.Approved);
        captured.PerformedByName.Should().Be(fixture.ManagerName);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsPending_ShouldSaveOnceAsync()
    {
        var fixture = new ApprovalFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsPending_ShouldInvalidateThreeCachesAsync()
    {
        var fixture = new ApprovalFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenPendingShipmentDoesNotExist_ShouldReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new ApprovalFixture();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(
                fixture.Shipment.Id, ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        fixture.Managers.Verify(x => x.GetUserIdAsync(
            It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenRepositoryReturnsNonPendingShipment_ShouldPropagateInvalidTransitionWithoutSavingAsync()
    {
        var fixture = new ApprovalFixture();
        fixture.Shipment.ApproveShipment(Guid.NewGuid());

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidShipmentStateException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var fixture = new ApprovalFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Shipments.Verify(x => x.GetShipmentInStatusAsync(
            fixture.Shipment.Id, ShipmentStatuses.Pending, cancellation.Token, false), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsPending_ShouldTransitionToRejectedAsync()
    {
        var fixture = new RejectionFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fixture.Shipment.CurrentStatus.Should().Be(ShipmentStatuses.Rejected);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsPending_ShouldPersistRejectionReasonAsync()
    {
        var fixture = new RejectionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Shipment.RejectionReason.Should().Be(fixture.Command.RejectionReason);
        fixture.Shipment.IsRejected.Should().BeTrue();
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsPending_ShouldCreateRejectedActivityLogAsync()
    {
        var fixture = new RejectionFixture();
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        captured!.Action.Should().Be(ActivityAction.Rejected);
        captured.Description.Should().Contain(fixture.Command.RejectionReason);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsPending_ShouldSaveOnceAsync()
    {
        var fixture = new RejectionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsPending_ShouldInvalidateThreeCachesAsync()
    {
        var fixture = new RejectionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenPendingShipmentDoesNotExist_ShouldReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new RejectionFixture();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(
                fixture.Shipment.Id, ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenRepositoryReturnsNonPendingShipment_ShouldPropagateInvalidTransitionWithoutSavingAsync()
    {
        var fixture = new RejectionFixture();
        fixture.Shipment.ApproveShipment(Guid.NewGuid());

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidShipmentStateException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var fixture = new RejectionFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Shipments.Verify(x => x.GetShipmentInStatusAsync(
            fixture.Shipment.Id, ShipmentStatuses.Pending, cancellation.Token, true), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    private sealed class ApprovalFixture
    {
        public Shipment Shipment { get; } = ShipmentTestData.CreateShipment();
        public Guid ManagerUserId { get; } = Guid.NewGuid();
        public Guid ManagerProfileId { get; } = Guid.NewGuid();
        public string ManagerName { get; } = "Nour Hassan";
        public ApproveShipmentCommand Command { get; }
        public Mock<IShipmentQueryRepository> Shipments { get; } = new();
        public Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public ApproveShipmentHandler Handler { get; }

        public ApprovalFixture()
        {
            Command = new ApproveShipmentCommand(Guid.NewGuid(), ManagerUserId, Shipment.Id);
            Shipments.Setup(x => x.GetShipmentInStatusAsync(
                    Shipment.Id, ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(Shipment);
            Managers.Setup(x => x.GetUserIdAsync(ManagerUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ManagerProfileId);
            Managers.Setup(x => x.GetOperationManagerNameAsync(ManagerUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ManagerName);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new ApproveShipmentHandler(
                Shipments.Object,
                Mock.Of<ILogger<ApproveShipmentHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
        }
    }

    private sealed class RejectionFixture
    {
        public Shipment Shipment { get; } = ShipmentTestData.CreateShipment();
        public Guid ManagerUserId { get; } = Guid.NewGuid();
        public Guid ManagerProfileId { get; } = Guid.NewGuid();
        public string ManagerName { get; } = "Nour Hassan";
        public RejectShipmentCommand Command { get; }
        public Mock<IShipmentQueryRepository> Shipments { get; } = new();
        public Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public RejectShipmentHandler Handler { get; }

        public RejectionFixture()
        {
            Command = new RejectShipmentCommand(Guid.NewGuid(), ManagerUserId, Shipment.Id, "Invalid address");
            Shipments.Setup(x => x.GetShipmentInStatusAsync(
                    Shipment.Id, ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(Shipment);
            Managers.Setup(x => x.GetUserIdAsync(ManagerUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ManagerProfileId);
            Managers.Setup(x => x.GetOperationManagerNameAsync(ManagerUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ManagerName);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new RejectShipmentHandler(
                Shipments.Object,
                Mock.Of<ILogger<RejectShipmentHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
        }
    }
}
