using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.TripService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.ApplicationLayer.Tests.Commands.Trips;

public sealed class TripStartWorkflowPhase2Tests
{
    [Fact]
    public async Task StartPickupTripHandler_WhenTripServiceSucceeds_ShouldReturnSuccess()
    {
        var fixture = new PickupTripFixture();
        (await fixture.Handler.Handle(fixture.Command, CancellationToken.None)).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenCalled_ShouldForwardManagerAndCarrierIds()
    {
        var fixture = new PickupTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Service.Verify(x => x.StartPickUpTripAsync(
            fixture.Command.OperationManagerId, fixture.Command.CarrierId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripStarts_ShouldResolveManagerDisplayName()
    {
        var fixture = new PickupTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Managers.Verify(x => x.GetOperationManagerNameAsync(
            fixture.Command.OperationManagerId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripStarts_ShouldWriteThreeActivityLogs()
    {
        var fixture = new PickupTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Logs.Verify(x => x.Log(It.IsAny<SystemActivityLog>(), CancellationToken.None), Times.Exactly(3));
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripStarts_ShouldLogCreatedAndStartedActions()
    {
        var fixture = new PickupTripFixture();
        var logs = new List<SystemActivityLog>();
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => logs.Add(log))
            .Returns(Task.CompletedTask);
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        logs.Select(x => x.Action).Should().Equal(ActivityAction.Created, ActivityAction.Started, ActivityAction.Started);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripStarts_ShouldUseResolvedActorForEveryLog()
    {
        var fixture = new PickupTripFixture();
        var logs = new List<SystemActivityLog>();
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => logs.Add(log))
            .Returns(Task.CompletedTask);
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        logs.Should().OnlyContain(x => x.PerformedByName == fixture.ManagerName);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripStarts_ShouldSaveOnce()
    {
        var fixture = new PickupTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripStarts_ShouldInvalidateFourCaches()
    {
        var fixture = new PickupTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(4));
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new PickupTripFixture();
        using var cancellation = new CancellationTokenSource();
        await fixture.Handler.Handle(fixture.Command, cancellation.Token);
        fixture.Service.Verify(x => x.StartPickUpTripAsync(
            fixture.Command.OperationManagerId, fixture.Command.CarrierId, cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenTripServiceFails_ShouldPropagateWithoutSaving()
    {
        var fixture = new PickupTripFixture();
        fixture.Service.Setup(x => x.StartPickUpTripAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("no eligible pickup shipments"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("no eligible pickup shipments");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenManagerNameLookupFails_ShouldNotSave()
    {
        var fixture = new PickupTripFixture();
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("manager missing"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenActivityLogFails_ShouldNotSave()
    {
        var fixture = new PickupTripFixture();
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("audit unavailable"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartPickupTripHandler_WhenSaveFails_ShouldNotInvalidateCaches()
    {
        var fixture = new PickupTripFixture();
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        fixture.Cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripServiceSucceeds_ShouldReturnSuccess()
    {
        var fixture = new DeliveryTripFixture();
        (await fixture.Handler.Handle(fixture.Command, CancellationToken.None)).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenCalled_ShouldForwardManagerAndCarrierIds()
    {
        var fixture = new DeliveryTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Service.Verify(x => x.StartDeliveryTripAsync(
            fixture.Command.OperationManagerId, fixture.Command.CarrierId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripStarts_ShouldResolveManagerDisplayName()
    {
        var fixture = new DeliveryTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Managers.Verify(x => x.GetOperationManagerNameAsync(
            fixture.Command.OperationManagerId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripStarts_ShouldWriteThreeActivityLogs()
    {
        var fixture = new DeliveryTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Logs.Verify(x => x.Log(It.IsAny<SystemActivityLog>(), CancellationToken.None), Times.Exactly(3));
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripStarts_ShouldLogTripAndShipmentEntities()
    {
        var fixture = new DeliveryTripFixture();
        var logs = new List<SystemActivityLog>();
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => logs.Add(log))
            .Returns(Task.CompletedTask);
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        logs.Select(x => x.EntityType).Should().Equal(
            ActivityEntityType.Trip, ActivityEntityType.Trip, ActivityEntityType.Shipment);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripStarts_ShouldUseResolvedActorForEveryLog()
    {
        var fixture = new DeliveryTripFixture();
        var logs = new List<SystemActivityLog>();
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => logs.Add(log))
            .Returns(Task.CompletedTask);
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        logs.Should().OnlyContain(x => x.PerformedByName == fixture.ManagerName);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripStarts_ShouldSaveOnce()
    {
        var fixture = new DeliveryTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripStarts_ShouldInvalidateFourCaches()
    {
        var fixture = new DeliveryTripFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(4));
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenCancellationTokenIsPassed_ShouldForwardIt()
    {
        var fixture = new DeliveryTripFixture();
        using var cancellation = new CancellationTokenSource();
        await fixture.Handler.Handle(fixture.Command, cancellation.Token);
        fixture.Service.Verify(x => x.StartDeliveryTripAsync(
            fixture.Command.OperationManagerId, fixture.Command.CarrierId, cancellation.Token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenTripServiceFails_ShouldPropagateWithoutSaving()
    {
        var fixture = new DeliveryTripFixture();
        fixture.Service.Setup(x => x.StartDeliveryTripAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("no eligible delivery shipments"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("no eligible delivery shipments");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenManagerNameLookupFails_ShouldNotSave()
    {
        var fixture = new DeliveryTripFixture();
        fixture.Managers.Setup(x => x.GetOperationManagerNameAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("manager missing"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenActivityLogFails_ShouldNotSave()
    {
        var fixture = new DeliveryTripFixture();
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("audit unavailable"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartDeliveryTripHandler_WhenSaveFails_ShouldNotInvalidateCaches()
    {
        var fixture = new DeliveryTripFixture();
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
        fixture.Cache.VerifyNoOtherCalls();
    }

    private static Trip PickupTrip(Guid carrierId)
    {
        var shipment = ShipmentTestData.CreateShipment();
        shipment.ApproveShipment(Guid.NewGuid());
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, carrierId, Guid.NewGuid());
        return Trip.Plan(carrierId, Guid.NewGuid(), TripType.Pickup, [shipment]);
    }

    private static Trip DeliveryTrip(Guid carrierId)
    {
        var shipment = ShipmentTestData.CreateShipment();
        shipment.ApproveShipment(Guid.NewGuid());
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, carrierId, Guid.NewGuid());
        shipment.AssignedAsPickUpTrip(Guid.NewGuid(), carrierId);
        shipment.DeliveredToWarehouse(carrierId);
        shipment.TripId = null;
        return Trip.Plan(carrierId, Guid.NewGuid(), TripType.Delivery, [shipment]);
    }

    private sealed class PickupTripFixture
    {
        public string ManagerName { get; } = "Nour Hassan";
        public StartPickUpTripCommand Command { get; } = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        public Trip Trip { get; }
        public Mock<ITripServices> Service { get; } = new();
        public Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public StartPickupTripHandler Handler { get; }
        public PickupTripFixture()
        {
            Trip = PickupTrip(Command.CarrierId);
            Service.Setup(x => x.StartPickUpTripAsync(
                    Command.OperationManagerId, Command.CarrierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Trip);
            Managers.Setup(x => x.GetOperationManagerNameAsync(
                    Command.OperationManagerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ManagerName);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new StartPickupTripHandler(
                Service.Object, Managers.Object, Logs.Object, UnitOfWork.Object, Cache.Object,
                Mock.Of<ILogger<StartPickupTripHandler>>());
        }
    }

    private sealed class DeliveryTripFixture
    {
        public string ManagerName { get; } = "Nour Hassan";
        public StartDeliveryTripCommand Command { get; } = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        public Trip Trip { get; }
        public Mock<ITripServices> Service { get; } = new();
        public Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        public Mock<ISystemLogCommands> Logs { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public StartDeliveryTripHandler Handler { get; }
        public DeliveryTripFixture()
        {
            Trip = DeliveryTrip(Command.CarrierId);
            Service.Setup(x => x.StartDeliveryTripAsync(
                    Command.OperationManagerId, Command.CarrierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Trip);
            Managers.Setup(x => x.GetOperationManagerNameAsync(
                    Command.OperationManagerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ManagerName);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new StartDeliveryTripHandler(
                Service.Object, Managers.Object, Logs.Object, UnitOfWork.Object, Cache.Object,
                Mock.Of<ILogger<StartDeliveryTripHandler>>());
        }
    }
}
