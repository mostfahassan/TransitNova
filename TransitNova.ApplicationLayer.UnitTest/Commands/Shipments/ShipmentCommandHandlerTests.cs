using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.SystemLogs;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class ShipmentCommandHandlerTests
{
    [Fact]
    public async Task CreateShipmentCommandHandler_WhenInputIsValid_ShouldReturnCreatedResultAndPersistActivityLog()
    {
        var fixture = new CreateFixture();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        var dto = new RetrieveShipmentDto { Id = shipmentId, TrackingNumber = "TN-100", ShippingCost = 125 };
        fixture.Service.Setup(x => x.PrepareShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, It.IsAny<CancellationToken>())).ReturnsAsync(shipmentId);
        fixture.Shipments.Setup(x => x.CreateShipmentForUserAsync(shipmentId, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        fixture.Users.Setup(x => x.FindByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(new AppUserDto { Id = userId, FullName = "Ahmed Ali" });
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? capturedLog = null;
        fixture.Logs.Setup(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => capturedLog = log)
            .Returns(Task.CompletedTask);
        var command = new CreateShipmentCommand(Guid.NewGuid(), ValidCreateDto(), userId);

        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Created);
        result.Data.Should().BeSameAs(dto);
        capturedLog.Should().NotBeNull();
        capturedLog!.Action.Should().Be(ActivityAction.Created);
        capturedLog.PerformedByUserId.Should().Be(userId);
        capturedLog.PerformedByName.Should().Be("Ahmed Ali");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        fixture.Cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(6));
    }

    [Fact]
    public async Task CreateShipmentCommandHandler_WhenCreatedShipmentCannotBeRetrieved_ShouldReturnFailureWithoutLogging()
    {
        var fixture = new CreateFixture();
        var shipmentId = Guid.NewGuid();
        fixture.Service.Setup(x => x.PrepareShipmentCreation(It.IsAny<CreateShipmentDto>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(shipmentId);
        fixture.Shipments.Setup(x => x.CreateShipmentForUserAsync(shipmentId, It.IsAny<CancellationToken>())).ReturnsAsync((RetrieveShipmentDto?)null);

        var result = await fixture.Handler.Handle(
            new CreateShipmentCommand(Guid.NewGuid(), ValidCreateDto(), Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Shipment created but retrieval failed");
        fixture.Users.VerifyNoOtherCalls();
        fixture.Logs.Verify(x => x.Log(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        fixture.Cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateShipmentCommandHandler_WhenCancellationTokenIsProvided_ShouldForwardTokenToDependencies()
    {
        var fixture = new CreateFixture();
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        fixture.Service.Setup(x => x.PrepareShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, token)).ReturnsAsync(shipmentId);
        fixture.Shipments.Setup(x => x.CreateShipmentForUserAsync(shipmentId, token)).ReturnsAsync(new RetrieveShipmentDto { Id = shipmentId, TrackingNumber = "TN-101" });
        fixture.Users.Setup(x => x.FindByIdAsync(userId, token)).ReturnsAsync(new AppUserDto { Id = userId, FullName = "Ahmed" });
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(token)).ReturnsAsync(1);

        await fixture.Handler.Handle(new CreateShipmentCommand(Guid.NewGuid(), ValidCreateDto(), userId), token);

        fixture.Service.Verify(x => x.PrepareShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, token), Times.Once);
        fixture.Shipments.Verify(x => x.CreateShipmentForUserAsync(shipmentId, token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    public async Task UpdateShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnNotFoundWithoutSaving()
    {
        var shipments = new Mock<IShipmentQueryRepository>();
        var service = new Mock<IShipmentService>();
        var cache = new Mock<ICacheService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var shipmentId = Guid.NewGuid();
        shipments.Setup(x => x.GetEntityAsync(shipmentId, It.IsAny<CancellationToken>())).ReturnsAsync((Shipment?)null);
        var handler = new UpdateShipmentHandler(Mock.Of<ILogger<UpdateShipmentHandler>>(), shipments.Object, service.Object, cache.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new UpdateShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), shipmentId, ValidUpdateDto()),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        service.Verify(x => x.UpdateShipmentDetails(It.IsAny<Shipment>(), It.IsAny<UpdateShipmentDto>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateShipmentHandler_WhenShipmentExists_ShouldUpdateSaveAndInvalidateCaches()
    {
        var shipment = ShipmentTestData.CreateShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var service = new Mock<IShipmentService>();
        var cache = new Mock<ICacheService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var updateDto = ValidUpdateDto();
        shipments.Setup(x => x.GetEntityAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new UpdateShipmentHandler(Mock.Of<ILogger<UpdateShipmentHandler>>(), shipments.Object, service.Object, cache.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new UpdateShipmentCommand(Guid.NewGuid(), userId, shipment.Id, updateDto),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        service.Verify(x => x.UpdateShipmentDetails(shipment, updateDto), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(7));
    }

    [Fact]
    public async Task UpdateShipmentHandler_WhenShipmentServiceThrows_ShouldPropagateExceptionWithoutSaving()
    {
        var shipment = ShipmentTestData.CreateShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var service = new Mock<IShipmentService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        shipments.Setup(x => x.GetEntityAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        service.Setup(x => x.UpdateShipmentDetails(shipment, It.IsAny<UpdateShipmentDto>())).Throws(new InvalidOperationException("invalid transition"));
        var handler = new UpdateShipmentHandler(Mock.Of<ILogger<UpdateShipmentHandler>>(), shipments.Object, service.Object, Mock.Of<ICacheService>(), unitOfWork.Object);

        var act = () => handler.Handle(
            new UpdateShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), shipment.Id, ValidUpdateDto()),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("invalid transition");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnFailureWithoutSaving()
    {
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        shipments.Setup(x => x.GetShipmentForCommands(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Shipment?)null);
        var handler = new DeleteShipmentHandler(shipments.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<DeleteShipmentHandler>>());

        var result = await handler.Handle(
            new DeleteShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteShipmentHandler_WhenShipmentExists_ShouldSoftDeleteSaveAndInvalidateCaches()
    {
        var shipment = ShipmentTestData.CreateDeliveredShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        shipments.Setup(x => x.GetShipmentForCommands(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new DeleteShipmentHandler(shipments.Object, unitOfWork.Object, cache.Object, Mock.Of<ILogger<DeleteShipmentHandler>>());

        var result = await handler.Handle(
            new DeleteShipmentCommand(Guid.NewGuid(), shipment.Id, Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.IsDeleted.Should().BeTrue();
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(7));
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnFailureWithoutSaving()
    {
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        shipments.Setup(x => x.GetShipmentForCommands(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Shipment?)null);
        var handler = new IssueShipmentHandler(shipments.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<IssueShipmentHandler>>());

        var result = await handler.Handle(
            new IssueShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Damaged package"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenShipmentExists_ShouldIssueSaveAndInvalidateCaches()
    {
        var shipment = ShipmentTestData.CreateDeliveredShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        shipments.Setup(x => x.GetShipmentForCommands(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new IssueShipmentHandler(shipments.Object, unitOfWork.Object, cache.Object, Mock.Of<ILogger<IssueShipmentHandler>>());

        var result = await handler.Handle(
            new IssueShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), shipment.Id, "Damaged package"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.CurrentStatus.Should().Be(TransitNova.Domain.Enums.Shipment.ShipmentStatuses.Issue);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(7));
    }

    private static CreateShipmentDto ValidCreateDto() => new(
        new CreateReceiverDto
        {
            FirstName = "Mona",
            LastName = "Ali",
            Email = "mona@example.com",
            PhoneNumber = "01000000000",
            Address = "Cairo",
            CityId = 1
        },
        new PackageSpecificationDto { Weight = 5, Width = 10, Height = 10, Length = 10 },
        TransitNova.Domain.Enums.Shipment.Currency.EGP,
        DateTime.UtcNow.AddDays(1),
        TransitNova.Domain.Enums.Shipment.TransportationMode.Land,
        TransitNova.Domain.Enums.Shipment.enShipmentType.Standard,
        "Delivery Address",
        "Pickup Address",
        null);

    private static UpdateShipmentDto ValidUpdateDto() => new(
        null,
        "New Delivery Address",
        "New Pickup Address",
        null,
        null,
        null);

    private sealed class CreateFixture
    {
        internal Mock<IShipmentQueryRepository> Shipments { get; } = new();
        internal Mock<IShipmentService> Service { get; } = new();
        internal Mock<IUserAuthQueryService> Users { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();
        internal Mock<ICacheService> Cache { get; } = new();
        internal CreateShipmentCommandHandler Handler { get; }

        internal CreateFixture()
        {
            Handler = new CreateShipmentCommandHandler(
                Shipments.Object,
                Service.Object,
                Users.Object,
                Logs.Object,
                UnitOfWork.Object,
                Cache.Object,
                Mock.Of<ILogger<CreateShipmentCommandHandler>>());
        }
    }
}
