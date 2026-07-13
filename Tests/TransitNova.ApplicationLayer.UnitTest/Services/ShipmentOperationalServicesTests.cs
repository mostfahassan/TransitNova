using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.CompleteShipments;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Services.CompleteShipmentService;
using TransitNova.BusinessLayer.Services.ShipmentAssignmentServices;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class ShipmentOperationalServicesTests
{
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task AssignPickupAsync_Should_ThrowNotFound_WhenShipmentOrCarrierIsMissingAsync(
        bool missingShipment,
        bool missingCarrier)
    {
        var fixture = new Fixture();
        fixture.Shipments.Setup(x => x.GetShipmentInStatusAsync(
                fixture.Shipment.Id, ShipmentStatuses.Approved, CancellationToken.None, true))
            .ReturnsAsync(missingShipment ? null : fixture.Shipment);
        fixture.Carriers.Setup(x => x.GetCarrierAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Carrier, bool>>>(), CancellationToken.None))
            .ReturnsAsync(missingCarrier ? null : fixture.Carrier);

        var act = () => fixture.AssignmentService().AssignPickupAsync(
            fixture.Shipment.Id, fixture.AppUserId, fixture.Carrier.Id, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task AssignPickupAsync_Should_AssignCarrierAndShipment_WhenInputsExistAsync()
    {
        var fixture = new Fixture();
        fixture.Shipment.ApproveShipment(fixture.ManagerProfileId);
        fixture.ArrangeAssignment(ShipmentStatuses.Approved);

        var tracking = await fixture.AssignmentService().AssignPickupAsync(
            fixture.Shipment.Id, fixture.AppUserId, fixture.Carrier.Id, CancellationToken.None);

        tracking.Should().Be(fixture.Shipment.TrackingNumber);
        fixture.Shipment.CurrentStatus.Should().Be(ShipmentStatuses.AssignedToPickUpCarrier);
        fixture.Carrier.Status.Should().Be(CarrierStatus.AssignedToPickUpShipment);
        fixture.Carrier.HandlerId.Should().Be(fixture.ManagerProfileId);
    }

    [Fact]
    public async Task AssignDeliveryAsync_Should_AssignCarrierAndWarehouseShipmentAsync()
    {
        var fixture = new Fixture();
        MoveToWarehouse(fixture.Shipment, fixture.ManagerProfileId, Guid.NewGuid());
        fixture.ArrangeAssignment(ShipmentStatuses.InWarehouse);

        var tracking = await fixture.AssignmentService().AssignDeliveryAsync(
            fixture.Shipment.Id, fixture.AppUserId, fixture.Carrier.Id, CancellationToken.None);

        tracking.Should().Be(fixture.Shipment.TrackingNumber);
        fixture.Shipment.CurrentStatus.Should().Be(ShipmentStatuses.AssignedToDeliveryCarrier);
        fixture.Carrier.Status.Should().Be(CarrierStatus.AssignedToDeliveryShipment);
    }

    [Fact]
    public async Task PickedUpShipmentAsync_Should_ThrowNotFound_WhenCarrierDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.Carriers.Setup(x => x.GetCarrierAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Carrier, bool>>>(), CancellationToken.None))
            .ReturnsAsync((Carrier?)null);

        var act = () => fixture.CompletionService().PickedUpShipmentAsync(
            fixture.Shipment.Id, fixture.Carrier.Id, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        fixture.Shipments.Verify(x => x.GetEntityAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PickedUpShipmentAsync_Should_ThrowNotFound_WhenShipmentDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.ArrangeCarrierLookup();
        fixture.Shipments.Setup(x => x.GetEntityAsync(fixture.Shipment.Id, CancellationToken.None)).ReturnsAsync((Shipment?)null);

        var act = () => fixture.CompletionService().PickedUpShipmentAsync(
            fixture.Shipment.Id, fixture.Carrier.Id, CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task PickedUpShipmentAsync_Should_UpdateStatusAndWriteAuditLogAsync()
    {
        var fixture = new Fixture();
        fixture.Shipment.ApproveShipment(fixture.ManagerProfileId);
        fixture.Shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, fixture.Carrier.Id, fixture.ManagerProfileId);
        fixture.Shipment.AssignedAsPickupTrip(Guid.NewGuid(), fixture.Carrier.Id);
        fixture.ArrangeCompletion();

        var result = await fixture.CompletionService().PickedUpShipmentAsync(
            fixture.Shipment.Id, fixture.Carrier.Id, CancellationToken.None);

        result.CurrentStatus.Should().Be(ShipmentStatuses.PickedUp);
        fixture.Logs.Verify(x => x.LogAsync(
            It.Is<SystemActivityLog>(log => log.Description.Contains(fixture.Shipment.Id.ToString(), StringComparison.Ordinal)),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CompleteShipmentDeliveryAsync_Should_CompleteCarrierAndShipmentAndLogAsync()
    {
        var fixture = new Fixture();
        fixture.Carrier.AssignToDeliver(fixture.ManagerProfileId);
        fixture.Shipment.ApproveShipment(fixture.ManagerProfileId);
        fixture.Shipment.AssignToCarrier(ShipmentStatuses.AssignedToDeliveryCarrier, fixture.Carrier.Id, fixture.ManagerProfileId);
        fixture.Shipment.AssignedAsDeliveryTrip(Guid.NewGuid(), fixture.Carrier.Id);
        fixture.ArrangeCompletion();

        var result = await fixture.CompletionService().CompleteShipmentDeliveryAsync(
            fixture.Shipment.Id, fixture.Carrier.Id, CancellationToken.None);

        result.CurrentStatus.Should().Be(ShipmentStatuses.Delivered);
        fixture.Carrier.CompletedShipmentsCount.Should().Be(1);
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CompleteShipmentToWarehouseAsync_Should_UpdateBothAggregatesWithoutLogAsync()
    {
        var fixture = new Fixture();
        fixture.Carrier.AssignToPickup(fixture.ManagerProfileId);
        fixture.Shipment.ApproveShipment(fixture.ManagerProfileId);
        fixture.Shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, fixture.Carrier.Id, fixture.ManagerProfileId);
        fixture.Shipment.AssignedAsPickupTrip(Guid.NewGuid(), fixture.Carrier.Id);
        fixture.ArrangeCompletion();

        var result = await fixture.CompletionService().CompleteShipmentToWarehouseAsync(
            fixture.Shipment.Id, fixture.Carrier.Id, CancellationToken.None);

        result.CurrentStatus.Should().Be(ShipmentStatuses.InWarehouse);
        fixture.Carrier.CompletedShipmentsCount.Should().Be(1);
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static void MoveToWarehouse(Shipment shipment, Guid managerId, Guid pickupCarrierId)
    {
        shipment.ApproveShipment(managerId);
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, pickupCarrierId, managerId);
        shipment.AssignedAsPickupTrip(Guid.NewGuid(), pickupCarrierId);
        shipment.DeliveredToWarehouse(pickupCarrierId);
    }

    private sealed class Fixture
    {
        internal Guid AppUserId { get; } = Guid.NewGuid();
        internal Guid ManagerProfileId { get; } = Guid.NewGuid();
        internal Shipment Shipment { get; } = ShipmentTestData.CreateShipment();
        internal Carrier Carrier { get; } = Carrier.Create(
            Guid.NewGuid(), "Ahmed", "Ali", "carrier@example.com", "+201001234567",
            Address.Create("Cairo", null, "Main Street"), 1);
        internal Mock<IShipmentQueryRepository> Shipments { get; } = new();
        internal Mock<ICarrierQueryRepository> Carriers { get; } = new();
        internal Mock<IOperationManagerQueryRepository> Managers { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();

        internal Fixture()
        {
            Carrier.AddAdditionalData(
                Carrier.AppUserId,
                "LIC-123",
                10,
                10m,
                5,
                DateTime.UtcNow.AddDays(1),
                2,
                Guid.NewGuid());
            Managers.Setup(x => x.GetUserIdAsync(AppUserId, CancellationToken.None)).ReturnsAsync(ManagerProfileId);
            Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        }

        internal void ArrangeAssignment(ShipmentStatuses status)
        {
            Shipments.Setup(x => x.GetShipmentInStatusAsync(Shipment.Id, status, CancellationToken.None, true)).ReturnsAsync(Shipment);
            ArrangeCarrierLookup();
        }

        internal void ArrangeCarrierLookup()
        {
            Carriers.Setup(x => x.GetCarrierAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Carrier, bool>>>(), CancellationToken.None)).ReturnsAsync(Carrier);
        }

        internal void ArrangeCompletion()
        {
            ArrangeCarrierLookup();
            Shipments.Setup(x => x.GetEntityAsync(Shipment.Id, CancellationToken.None)).ReturnsAsync(Shipment);
            Carriers.Setup(x => x.GetCarrierNameAsync(Carrier.Id, CancellationToken.None)).ReturnsAsync(Carrier.FullName);
        }

        internal ShipmentAssignmentService AssignmentService() => new(
            Shipments.Object,
            Carriers.Object,
            Managers.Object,
            NullLogger<AssignShipmentPickupToCarrierHandler>.Instance);

        internal CompleteShipmentService CompletionService() => new(
            Carriers.Object,
            Shipments.Object,
            Logs.Object,
            NullLogger<CompleteShipmentToWarehouseHandler>.Instance);
    }
}

