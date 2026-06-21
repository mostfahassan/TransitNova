using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentAssignmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class AssignmentWorkflowPhase2Tests
{
    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentServiceSucceeds_ShouldReturnSuccess()
    {
        var fixture = new PickupAssignmentFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenCalled_ShouldForwardAllAssignmentIdentifiers()
    {
        var fixture = new PickupAssignmentFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Assignment.Verify(x => x.AssignPickup(
            fixture.Command.ShipmentId,
            fixture.Command.OperationManagerId,
            fixture.Command.CarrierId,
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentSucceeds_ShouldResolveManagerName()
    {
        var fixture = new PickupAssignmentFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Managers.Verify(x => x.GetOperationManagerNameAsync(
            fixture.Command.OperationManagerId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentSucceeds_ShouldCreateAssignedActivityLog()
    {
        var fixture = new PickupAssignmentFixture();
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        captured!.Action.Should().Be(ActivityAction.Assigned);
        captured.Description.Should().Contain(fixture.TrackingNumber);
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentSucceeds_ShouldSaveAndInvalidateSixCaches()
    {
        var fixture = new PickupAssignmentFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(6));
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenAssignmentFails_ShouldPropagateWithoutSaving()
    {
        var fixture = new PickupAssignmentFixture();
        fixture.Assignment.Setup(x => x.AssignPickup(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("carrier unavailable"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("carrier unavailable");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignShipmentPickUpToCarrierHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new PickupAssignmentFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Assignment.Verify(x => x.AssignPickup(
            fixture.Command.ShipmentId,
            fixture.Command.OperationManagerId,
            fixture.Command.CarrierId,
            cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenAssignmentServiceSucceeds_ShouldReturnSuccess()
    {
        var fixture = new DeliveryAssignmentFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenCalled_ShouldForwardAllAssignmentIdentifiers()
    {
        var fixture = new DeliveryAssignmentFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Assignment.Verify(x => x.AssignDelivery(
            fixture.Command.ShipmentId,
            fixture.Command.OperationManagerId,
            fixture.Command.CarrierId,
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenAssignmentSucceeds_ShouldResolveManagerName()
    {
        var fixture = new DeliveryAssignmentFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Managers.Verify(x => x.GetOperationManagerNameAsync(
            fixture.Command.OperationManagerId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenAssignmentSucceeds_ShouldCreateAssignedActivityLog()
    {
        var fixture = new DeliveryAssignmentFixture();
        SystemActivityLog? captured = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => captured = log)
            .Returns(Task.CompletedTask);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        captured!.Action.Should().Be(ActivityAction.Assigned);
        captured.Description.Should().Contain(fixture.TrackingNumber);
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenAssignmentSucceeds_ShouldSaveAndInvalidateSixCaches()
    {
        var fixture = new DeliveryAssignmentFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(6));
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenAssignmentFails_ShouldPropagateWithoutSaving()
    {
        var fixture = new DeliveryAssignmentFixture();
        fixture.Assignment.Setup(x => x.AssignDelivery(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("shipment not in warehouse"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("shipment not in warehouse");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignShipmentDeliveryToCarrierHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new DeliveryAssignmentFixture();
        using var cancellation = new CancellationTokenSource();

        await fixture.Handler.Handle(fixture.Command, cancellation.Token);

        fixture.Assignment.Verify(x => x.AssignDelivery(
            fixture.Command.ShipmentId,
            fixture.Command.OperationManagerId,
            fixture.Command.CarrierId,
            cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    private sealed class PickupAssignmentFixture
    {
        public string TrackingNumber { get; } = "TRK-PICKUP";
        public AssignShipmentPickUpToCarrierCommand Command { get; } = new(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        public Mock<IShipmentAssignmentService> Assignment { get; } = new();
        public Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public AssignShipmentPickUpToCarrierHandler Handler { get; }

        public PickupAssignmentFixture()
        {
            Assignment.Setup(x => x.AssignPickup(
                    Command.ShipmentId, Command.OperationManagerId, Command.CarrierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TrackingNumber);
            Managers.Setup(x => x.GetOperationManagerNameAsync(
                    Command.OperationManagerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("Nour Hassan");
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new AssignShipmentPickUpToCarrierHandler(
                Assignment.Object,
                Mock.Of<ILogger<AssignShipmentPickUpToCarrierHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
        }
    }

    private sealed class DeliveryAssignmentFixture
    {
        public string TrackingNumber { get; } = "TRK-DELIVERY";
        public AssignShipmentDeliveryToCarrierCommand Command { get; } = new(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        public Mock<IShipmentAssignmentService> Assignment { get; } = new();
        public Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public AssignShipmentDeliveryToCarrierHandler Handler { get; }

        public DeliveryAssignmentFixture()
        {
            Assignment.Setup(x => x.AssignDelivery(
                    Command.ShipmentId, Command.OperationManagerId, Command.CarrierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TrackingNumber);
            Managers.Setup(x => x.GetOperationManagerNameAsync(
                    Command.OperationManagerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("Nour Hassan");
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new AssignShipmentDeliveryToCarrierHandler(
                Assignment.Object,
                Mock.Of<ILogger<AssignShipmentDeliveryToCarrierHandler>>(),
                Managers.Object,
                Logs.Object,
                Cache.Object,
                UnitOfWork.Object);
        }
    }
}
