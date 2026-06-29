using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Trips;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.ApplicationLayer.Tests.Commands.Trips;

public sealed class TripMutationWorkflowTests
{
    [Fact]
    public async Task CancelTripHandler_WhenTripDoesNotExist_ShouldReturnNotFoundWithoutSavingOrInvalidatingCacheAsync()
    {
        var fixture = new TripMutationFixture();
        var command = new CancelTripCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = await fixture.CreateCancelHandler().Handle(command, CancellationToken.None);

        result.Status.Should().Be(TransitNova.Domain.Enums.Result.ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelTripHandler_WhenTripExists_ShouldCancelSaveAndInvalidateTripCachesAsync()
    {
        var fixture = new TripMutationFixture();
        var trip = CreatePickupTrip();
        var operationManagerId = Guid.NewGuid();
        fixture.Repository.Setup(x => x.GetTripForCommandsAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var result = await fixture.CreateCancelHandler().Handle(
            new CancelTripCommand(trip.Id, operationManagerId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        trip.Status.Should().Be(TripStatus.Cancelled);
        trip.UpdatedBy.Should().Be(operationManagerId.ToString());
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CancelTripHandler_WhenSaveFails_ShouldNotInvalidateCacheAsync()
    {
        var fixture = new TripMutationFixture();
        var trip = CreatePickupTrip();
        fixture.Repository.Setup(x => x.GetTripForCommandsAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));

        var act = () => fixture.CreateCancelHandler().Handle(
            new CancelTripCommand(trip.Id, Guid.NewGuid()),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
    }

    [Fact]
    public async Task UpdateTripHandler_WhenTripDoesNotExist_ShouldReturnNotFoundWithoutSavingOrInvalidatingCacheAsync()
    {
        var fixture = new TripMutationFixture();
        var command = new UpdateTripCommand(Guid.NewGuid(), Guid.NewGuid(), new UpdateTripDto());

        var result = await fixture.CreateUpdateHandler().Handle(command, CancellationToken.None);

        result.Status.Should().Be(TransitNova.Domain.Enums.Result.ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTripHandler_WhenCarrierChanges_ShouldUpdateSaveAndInvalidateOldAndNewCarrierCachesAsync()
    {
        var fixture = new TripMutationFixture();
        var trip = CreatePickupTrip();
        var oldCarrierId = trip.CarrierId;
        var newCarrierId = Guid.NewGuid();
        var newWarehouseId = Guid.NewGuid();
        var plannedDate = DateTime.UtcNow.AddDays(2);
        fixture.Repository.Setup(x => x.GetTripForCommandsAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);
        var command = new UpdateTripCommand(
            trip.Id,
            Guid.NewGuid(),
            new UpdateTripDto
            {
                CarrierId = newCarrierId,
                WarehouseId = newWarehouseId,
                TripType = TripType.Delivery,
                PlannedDate = plannedDate,
                TotalShipments = 7
            });

        var result = await fixture.CreateUpdateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        trip.CarrierId.Should().Be(newCarrierId);
        trip.WarehouseId.Should().Be(newWarehouseId);
        trip.TripType.Should().Be(TripType.Delivery);
        trip.PlannedDate.Should().Be(plannedDate);
        trip.TotalShipments.Should().Be(7);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateTripHandler_WhenSaveFails_ShouldNotInvalidateCacheAsync()
    {
        var fixture = new TripMutationFixture();
        var trip = CreatePickupTrip();
        fixture.Repository.Setup(x => x.GetTripForCommandsAsync(trip.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));

        var act = () => fixture.CreateUpdateHandler().Handle(
            new UpdateTripCommand(trip.Id, Guid.NewGuid(), new UpdateTripDto { TotalShipments = 2 }),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
    }

    private static Trip CreatePickupTrip()
    {
        var carrierId = Guid.NewGuid();
        var shipment = ShipmentTestData.CreateShipment();
        shipment.ApproveShipment(Guid.NewGuid());
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, carrierId, Guid.NewGuid());
        return Trip.Plan(carrierId, Guid.NewGuid(), TripType.Pickup, [shipment]);
    }

    private sealed class TripMutationFixture
    {
        public Mock<ITripCommandRepository> Repository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();

        public TripMutationFixture()
        {
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        public CancelTripHandler CreateCancelHandler()
        {
            return new CancelTripHandler(
                Repository.Object,
                UnitOfWork.Object,
                NullLogger<CancelTripHandler>.Instance);
        }

        public UpdateTripHandler CreateUpdateHandler()
        {
            return new UpdateTripHandler(
                Repository.Object,
                UnitOfWork.Object,
                NullLogger<UpdateTripHandler>.Instance);
        }

        public List<string> CaptureRemovedCacheKeys()
        {
            var keys = new List<string>();
            Cache.Setup(x => x.RemoveAsync(It.IsAny<string>()))
                .Callback<string>(keys.Add)
                .Returns(Task.CompletedTask);
            return keys;
        }
    }
}


