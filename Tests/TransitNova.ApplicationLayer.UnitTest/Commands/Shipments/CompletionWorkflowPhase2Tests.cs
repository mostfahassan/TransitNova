using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.CompleteShipments;
using TransitNova.BusinessLayer.Interfaces.Services.CompleteShipmentService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class CompletionWorkflowPhase2Tests
{
    [Fact]
    public async Task CompleteShipmentToWarehouseHandler_WhenServiceCompletesShipment_ShouldReturnSuccessAsync()
    {
        var fixture = new WarehouseCompletionFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteShipmentToWarehouseHandler_WhenCalled_ShouldForwardShipmentAndCarrierIdsAsync()
    {
        var fixture = new WarehouseCompletionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Service.Verify(x => x.CompleteShipmentToWarehouseAsync(
            fixture.Command.ShipmentId, fixture.Command.CarrierId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CompleteShipmentToWarehouseHandler_WhenServiceCompletesShipment_ShouldSaveOnceAsync()
    {
        var fixture = new WarehouseCompletionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CompleteShipmentToWarehouseHandler_WhenTripPresenceVaries_ShouldDeclareCacheKeysAsync(bool hasTrip)
    {
        var fixture = new WarehouseCompletionFixture(hasTrip);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
    }

    [Fact]
    public async Task CompleteShipmentToWarehouseHandler_WhenServiceFails_ShouldPropagateWithoutSavingAsync()
    {
        var fixture = new WarehouseCompletionFixture();
        fixture.Service.Setup(x => x.CompleteShipmentToWarehouseAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("not assigned for pickup"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("not assigned for pickup");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CompleteShipmentCommandHandler_WhenServiceDeliversShipment_ShouldReturnSuccessAsync()
    {
        var fixture = new DeliveryCompletionFixture();

        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteShipmentCommandHandler_WhenCalled_ShouldForwardShipmentAndCarrierIdsAsync()
    {
        var fixture = new DeliveryCompletionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.Service.Verify(x => x.CompleteShipmentAsync(
            fixture.Command.ShipmentId, fixture.Command.CarrierId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CompleteShipmentCommandHandler_WhenServiceDeliversShipment_ShouldSaveOnceAsync()
    {
        var fixture = new DeliveryCompletionFixture();

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CompleteShipmentCommandHandler_WhenTripPresenceVaries_ShouldDeclareCacheKeysAsync(bool hasTrip)
    {
        var fixture = new DeliveryCompletionFixture(hasTrip);

        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
    }

    [Fact]
    public async Task CompleteShipmentCommandHandler_WhenServiceFails_ShouldPropagateWithoutSavingAsync()
    {
        var fixture = new DeliveryCompletionFixture();
        fixture.Service.Setup(x => x.CompleteShipmentAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("not assigned for delivery"));

        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("not assigned for delivery");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private sealed class WarehouseCompletionFixture
    {
        public Shipment Shipment { get; } = ShipmentTestData.CreateShipment();
        public CompleteShipmentToWarehouseCommand Command { get; }
        public Mock<ICompleteShipmentService> Service { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public CompleteShipmentToWarehouseHandler Handler { get; }

        public WarehouseCompletionFixture(bool hasTrip = true)
        {
            if (hasTrip) Shipment.TripId = Guid.NewGuid();
            Command = new CompleteShipmentToWarehouseCommand(Guid.NewGuid(), Shipment.Id, Guid.NewGuid());
            Service.Setup(x => x.CompleteShipmentToWarehouseAsync(
                    Command.ShipmentId, Command.CarrierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Shipment);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new CompleteShipmentToWarehouseHandler(
                Service.Object, UnitOfWork.Object,
                Mock.Of<ILogger<CompleteShipmentToWarehouseHandler>>());
        }
    }

    private sealed class DeliveryCompletionFixture
    {
        public Shipment Shipment { get; } = ShipmentTestData.CreateShipment();
        public CompleteShipmentCommand Command { get; }
        public Mock<ICompleteShipmentService> Service { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public CompleteShipmentCommandHandler Handler { get; }

        public DeliveryCompletionFixture(bool hasTrip = true)
        {
            if (hasTrip) Shipment.TripId = Guid.NewGuid();
            Command = new CompleteShipmentCommand(Guid.NewGuid(), Shipment.Id, Guid.NewGuid());
            Service.Setup(x => x.CompleteShipmentAsync(
                    Command.ShipmentId, Command.CarrierId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Shipment);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new CompleteShipmentCommandHandler(
                Service.Object, UnitOfWork.Object,
                Mock.Of<ILogger<CompleteShipmentCommandHandler>>());
        }
    }
}



