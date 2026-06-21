using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.ApplicationLayer.Tests.Commands.OperationManager;

public sealed class ShipmentWorkflowCommandHandlerTests
{
    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsNotPending_ShouldReturnNotFoundWithoutSaving()
    {
        var fixture = new ReviewFixture();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(
                It.IsAny<Guid>(), ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.ApproveHandler.Handle(
            new ApproveShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.Managers.VerifyNoOtherCalls();
        fixture.Logs.Verify(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApproveShipmentHandler_WhenShipmentIsPending_ShouldApproveLogSaveAndInvalidateCaches()
    {
        var fixture = new ReviewFixture();
        var shipment = ShipmentTestData.CreateShipment();
        var managerAppUserId = Guid.NewGuid();
        var managerProfileId = Guid.NewGuid();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(shipment.Id, ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), false)).ReturnsAsync(shipment);
        fixture.Managers.Setup(x => x.GetUserIdAsync(managerAppUserId, It.IsAny<CancellationToken>())).ReturnsAsync(managerProfileId);
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(managerAppUserId, It.IsAny<CancellationToken>())).ReturnsAsync("Omar Manager");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        var result = await fixture.ApproveHandler.Handle(
            new ApproveShipmentCommand(Guid.NewGuid(), managerAppUserId, shipment.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Approved);
        shipment.HandledById.Should().Be(managerProfileId);
        captured!.Action.Should().Be(ActivityAction.Approved);
        captured.PerformedByName.Should().Be("Omar Manager");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsNotPending_ShouldReturnNotFoundWithoutSaving()
    {
        var fixture = new ReviewFixture();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(
                It.IsAny<Guid>(), ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.RejectHandler.Handle(
            new RejectShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Invalid address"),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.Logs.Verify(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RejectShipmentHandler_WhenShipmentIsPending_ShouldRejectLogSaveAndInvalidateCaches()
    {
        var fixture = new ReviewFixture();
        var shipment = ShipmentTestData.CreateShipment();
        var managerAppUserId = Guid.NewGuid();
        var managerProfileId = Guid.NewGuid();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(shipment.Id, ShipmentStatuses.Pending, It.IsAny<CancellationToken>(), true)).ReturnsAsync(shipment);
        fixture.Managers.Setup(x => x.GetUserIdAsync(managerAppUserId, It.IsAny<CancellationToken>())).ReturnsAsync(managerProfileId);
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(managerAppUserId, It.IsAny<CancellationToken>())).ReturnsAsync("Omar Manager");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        var result = await fixture.RejectHandler.Handle(
            new RejectShipmentCommand(Guid.NewGuid(), managerAppUserId, shipment.Id, "Invalid address"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.CurrentStatus.Should().Be(ShipmentStatuses.Rejected);
        shipment.RejectionReason.Should().Be("Invalid address");
        captured!.Action.Should().Be(ActivityAction.Rejected);
        captured.Description.Should().Contain("Invalid address");
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentSucceeds_ShouldLogSaveAndInvalidateCaches()
    {
        var fixture = new AssignmentFixture();
        var shipmentId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        fixture.Assignment.Setup(x => x.AssignPickup(shipmentId, managerId, carrierId, It.IsAny<CancellationToken>())).ReturnsAsync("TN-PICKUP");
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(managerId, It.IsAny<CancellationToken>())).ReturnsAsync("Manager Name");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        var result = await fixture.PickupHandler.Handle(
            new AssignShipmentPickUpToCarrierCommand(Guid.NewGuid(), shipmentId, managerId, carrierId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.Action.Should().Be(ActivityAction.Assigned);
        captured.Description.Should().Contain("TN-PICKUP").And.Contain(carrierId.ToString());
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(6));
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentThrows_ShouldPropagateWithoutLoggingOrSaving()
    {
        var fixture = new AssignmentFixture();
        fixture.Assignment.Setup(x => x.AssignPickup(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("assignment failed"));

        var act = () => fixture.PickupHandler.Handle(
            new AssignShipmentPickUpToCarrierCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("assignment failed");
        fixture.Logs.Verify(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenAssignmentSucceeds_ShouldLogSaveAndInvalidateCaches()
    {
        var fixture = new AssignmentFixture();
        var shipmentId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        fixture.Assignment.Setup(x => x.AssignDelivery(shipmentId, managerId, carrierId, It.IsAny<CancellationToken>())).ReturnsAsync("TN-DELIVERY");
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(managerId, It.IsAny<CancellationToken>())).ReturnsAsync("Manager Name");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        var result = await fixture.DeliveryHandler.Handle(
            new AssignShipmentDeliveryToCarrierCommand(Guid.NewGuid(), shipmentId, managerId, carrierId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.Description.Should().Contain("TN-DELIVERY");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(6));
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenCancellationTokenIsProvided_ShouldForwardTokenToServiceAndPersistence()
    {
        var fixture = new AssignmentFixture();
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var shipmentId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        fixture.Assignment.Setup(x => x.AssignDelivery(shipmentId, managerId, carrierId, token)).ReturnsAsync("TN-DELIVERY");
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(managerId, token)).ReturnsAsync("Manager Name");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(token)).ReturnsAsync(1);

        await fixture.DeliveryHandler.Handle(
            new AssignShipmentDeliveryToCarrierCommand(Guid.NewGuid(), shipmentId, managerId, carrierId),
            token);

        fixture.Assignment.Verify(x => x.AssignDelivery(shipmentId, managerId, carrierId, token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    private sealed class ReviewFixture
    {
        internal Mock<IShipmentQueryRepository> Shipments { get; } = new();
        internal Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();
        internal Mock<ICacheService> Cache { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();
        internal ApproveShipmentHandler ApproveHandler { get; }
        internal RejectShipmentHandler RejectHandler { get; }

        internal ReviewFixture()
        {
            ApproveHandler = new ApproveShipmentHandler(
                Shipments.Object,
                Mock.Of<ILogger<ApproveShipmentHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
            RejectHandler = new RejectShipmentHandler(
                Shipments.Object,
                Mock.Of<ILogger<RejectShipmentHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
        }
    }

    private sealed class AssignmentFixture
    {
        internal Mock<IShipmentAssignmentService> Assignment { get; } = new();
        internal Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();
        internal Mock<ICacheService> Cache { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();
        internal AssignShipmentPickUpToCarrierHandler PickupHandler { get; }
        internal AssignShipmentDeliveryToCarrierHandler DeliveryHandler { get; }

        internal AssignmentFixture()
        {
            PickupHandler = new AssignShipmentPickUpToCarrierHandler(
                Assignment.Object,
                Mock.Of<ILogger<AssignShipmentPickUpToCarrierHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
            DeliveryHandler = new AssignShipmentDeliveryToCarrierHandler(
                Assignment.Object,
                Mock.Of<ILogger<AssignShipmentDeliveryToCarrierHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
        }
    }
}
