using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.ApplicationLayer.Tests.Handlers;

public sealed class CancelShipmentHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnNotFoundAndSkipWrites_When_ShipmentDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.Shipments.Setup(x => x.GetShipmentForCommandsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shipment?)null);

        var result = await fixture.Handler.Handle(
            new CancelShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        fixture.Cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_Should_CancelLogPersistAndInvalidateCaches_When_ShipmentExistsAsync()
    {
        var fixture = new Fixture();
        var shipment = CreateShipment();
        var userId = Guid.NewGuid();
        fixture.Shipments.Setup(x => x.GetShipmentForCommandsAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        fixture.Users.Setup(x => x.FindByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUserDto { Id = userId, FullName = "Ahmed Ali" });
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? capturedLog = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => capturedLog = log)
            .Returns(Task.CompletedTask);

        var result = await fixture.Handler.Handle(
            new CancelShipmentCommand(Guid.NewGuid(), userId, shipment.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.IsCancelled.Should().BeTrue();
        capturedLog.Should().NotBeNull();
        capturedLog!.Action.Should().Be(ActivityAction.Cancelled);
        capturedLog.PerformedByUserId.Should().Be(userId);
        capturedLog.PerformedByName.Should().Be("Ahmed Ali");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowDomainException_When_PersistenceReturnsNegativeResultAsync()
    {
        var fixture = new Fixture();
        var shipment = CreateShipment();
        var userId = Guid.NewGuid();
        fixture.Shipments.Setup(x => x.GetShipmentForCommandsAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        fixture.Users.Setup(x => x.FindByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AppUserDto { Id = userId, FullName = "Ahmed Ali" });
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(-1);

        var act = () => fixture.Handler.Handle(
            new CancelShipmentCommand(Guid.NewGuid(), userId, shipment.Id),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainOperationException>()
            .Where(e => e.ErrorCode == "SHIPMENT_CANCELLATION_FAILED");
    }

    private static Shipment CreateShipment()
    {
        var senderId = Guid.NewGuid();
        var receiver = ReceiverProfile.Create("Mona", "Ali", "mona@example.com", "0100", "Cairo", 1, senderId);
        return Shipment.Create(
            senderId,
            receiver,
            new Domain.Entities.Common.PackageSpecification(1m, 1m, 1m, 1m),
            TransitNova.Domain.Enums.Shipment.Currency.EGP,
            null,
            "Delivery",
            "Pickup",
            TransitNova.Domain.Enums.Shipment.enShipmentType.Standard,
            TransitNova.Domain.Enums.Shipment.TransportationMode.Land,
            null,
            Guid.NewGuid(),
            PaymentMethod.MobileWallets,
            100m,
            DateTime.UtcNow.AddDays(2));
    }

    private sealed class Fixture
    {
        internal Mock<IShipmentQueryRepository> Shipments { get; } = new();
        internal Mock<IUserAuthQueryService> Users { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();
        internal Mock<ICacheService> Cache { get; } = new();

        internal CancelShipmentHandler Handler { get; }

        internal Fixture()
        {
            Handler = new CancelShipmentHandler(
                Shipments.Object,
                Users.Object,
                Logs.Object,
                UnitOfWork.Object,
                Mock.Of<ILogger<CancelShipmentHandler>>());
        }
    }
}


