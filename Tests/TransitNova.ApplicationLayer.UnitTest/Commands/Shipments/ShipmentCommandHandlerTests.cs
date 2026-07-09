using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.SystemLogs;
using static TransitNova.Domain.Contracts.Caching.CacheKeys;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class ShipmentCommandHandlerTests
{
    [Fact]
    public async Task CreateShipmentCommandHandler_WhenInputIsValid_ShouldReturnCreatedInvoiceAndPersistActivityLogAsync()
    {
        var fixture = new CreateFixture();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        var invoice = new InvoiceDto
        {
            ShipmentId = shipmentId,
            PaymentId = Guid.NewGuid(),
            ShippingCost = 125,
            Commission = 12.5m,
            TotalAmount = 137.5m,
            PaymentMethod = PaymentMethod.PayPal,
            Status = PaymentStatus.Success,
            PaidAt = DateTime.UtcNow
        };

        fixture.Service.Setup(x => x.HandleShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Result<InvoiceDto>.Success(invoice), "TN-100"));
        fixture.Users.Setup(x => x.GetUserFullName(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("Ahmed Ali");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? capturedLog = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((log, _) => capturedLog = log)
            .Returns(Task.CompletedTask);
        var command = new CreateShipmentCommand(Guid.NewGuid(), ValidCreateDto(), userId);

        var result = await fixture.Handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Created);
        result.Data.Should().NotBeNull();
        result.Data!.ShipmentId.Should().Be(shipmentId);
        result.Data.PaymentId.Should().Be(invoice.PaymentId);
        result.Data.TotalAmount.Should().Be(invoice.TotalAmount);
        result.Data.PaymentMethod.Should().Be(PaymentMethod.PayPal);
        result.Data.Status.Should().Be(PaymentStatus.Success);
        capturedLog.Should().NotBeNull();
        capturedLog!.Action.Should().Be(ActivityAction.Created);
        capturedLog.PerformedByUserId.Should().Be(userId);
        capturedLog.PerformedByName.Should().Be("Ahmed Ali");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateShipmentCommandHandler_WhenInvoiceIsMissing_ShouldReturnFailureWithoutLoggingAsync()
    {
        var fixture = new CreateFixture();
        var userId = Guid.NewGuid();
        fixture.Service.Setup(x => x.HandleShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Result<InvoiceDto>.Failure(Errors.FailedOperation("Payment operation failed")), string.Empty));

        var result = await fixture.Handler.Handle(
            new CreateShipmentCommand(Guid.NewGuid(), ValidCreateDto(), userId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Failed to create shipment.");
        fixture.Users.VerifyNoOtherCalls();
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        fixture.Cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateShipmentCommandHandler_WhenCancellationTokenIsProvided_ShouldForwardTokenToDependenciesAsync()
    {
        var fixture = new CreateFixture();
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var userId = Guid.NewGuid();
        var invoice = new InvoiceDto { ShipmentId = Guid.NewGuid(), ShippingCost = 125 };
        fixture.Service.Setup(x => x.HandleShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, token))
            .ReturnsAsync((Result<InvoiceDto>.Success(invoice), "TN-100"));
        fixture.Users.Setup(x => x.GetUserFullName(userId, token))
            .ReturnsAsync("Ahmed");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(token)).ReturnsAsync(1);
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), token)).Returns(Task.CompletedTask);

        await fixture.Handler.Handle(new CreateShipmentCommand(Guid.NewGuid(), ValidCreateDto(), userId), token);

        fixture.Service.Verify(x => x.HandleShipmentCreation(It.IsAny<CreateShipmentDto>(), userId, token), Times.Once);
        fixture.Users.Verify(x => x.GetUserFullName(userId, token), Times.Exactly(2));
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), token), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }
    [Fact]
    public async Task UpdateShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnNotFoundWithoutSavingAsync()
    {
        var shipments = new Mock<IShipmentQueryRepository>();
        var service = new Mock<IShipmentService>();
        var cache = new Mock<ICacheService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var shipmentId = Guid.NewGuid();
        shipments.Setup(x => x.GetEntityAsync(shipmentId, It.IsAny<CancellationToken>())).ReturnsAsync((Shipment?)null);
        var handler = new UpdateShipmentHandler(Mock.Of<ILogger<UpdateShipmentHandler>>(), shipments.Object, service.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new UpdateShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), shipmentId, ValidUpdateDto()),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        service.Verify(x => x.UpdateShipmentDetails(It.IsAny<Shipment>(), It.IsAny<UpdateShipmentDto>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateShipmentHandler_WhenShipmentExists_ShouldUpdateSaveAndInvalidateCachesAsync()
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
        var handler = new UpdateShipmentHandler(Mock.Of<ILogger<UpdateShipmentHandler>>(), shipments.Object, service.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new UpdateShipmentCommand(Guid.NewGuid(), userId, shipment.Id, updateDto),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        service.Verify(x => x.UpdateShipmentDetails(shipment, updateDto), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateShipmentHandler_WhenShipmentServiceThrows_ShouldPropagateExceptionWithoutSavingAsync()
    {
        var shipment = ShipmentTestData.CreateShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var service = new Mock<IShipmentService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        shipments.Setup(x => x.GetEntityAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        service.Setup(x => x.UpdateShipmentDetails(shipment, It.IsAny<UpdateShipmentDto>())).Throws(new InvalidOperationException("invalid transition"));
        var handler = new UpdateShipmentHandler(Mock.Of<ILogger<UpdateShipmentHandler>>(), shipments.Object, service.Object, unitOfWork.Object);

        var act = () => handler.Handle(
            new UpdateShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), shipment.Id, ValidUpdateDto()),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("invalid transition");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnFailureWithoutSavingAsync()
    {
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        shipments.Setup(x => x.GetShipmentForCommandsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Shipment?)null);
        var handler = new DeleteShipmentHandler(shipments.Object, unitOfWork.Object, Mock.Of<ILogger<DeleteShipmentHandler>>());

        var result = await handler.Handle(
            new DeleteShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteShipmentHandler_WhenShipmentExists_ShouldSoftDeleteSaveAndInvalidateCachesAsync()
    {
        var shipment = ShipmentTestData.CreateDeliveredShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        shipments.Setup(x => x.GetShipmentForCommandsAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new DeleteShipmentHandler(shipments.Object, unitOfWork.Object, Mock.Of<ILogger<DeleteShipmentHandler>>());

        var result = await handler.Handle(
            new DeleteShipmentCommand(Guid.NewGuid(), shipment.Id, Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.IsDeleted.Should().BeTrue();
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenShipmentDoesNotExist_ShouldReturnFailureWithoutSavingAsync()
    {
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        shipments.Setup(x => x.GetShipmentForCommandsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Shipment?)null);
        var handler = new IssueShipmentHandler(shipments.Object, unitOfWork.Object, Mock.Of<ILogger<IssueShipmentHandler>>());

        var result = await handler.Handle(
            new IssueShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Damaged package"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IssueShipmentHandler_WhenShipmentExists_ShouldIssueSaveAndInvalidateCachesAsync()
    {
        var shipment = ShipmentTestData.CreateDeliveredShipment();
        var shipments = new Mock<IShipmentQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        shipments.Setup(x => x.GetShipmentForCommandsAsync(shipment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(shipment);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new IssueShipmentHandler(shipments.Object, unitOfWork.Object, Mock.Of<ILogger<IssueShipmentHandler>>());

        var result = await handler.Handle(
            new IssueShipmentCommand(Guid.NewGuid(), Guid.NewGuid(), shipment.Id, "Damaged package"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shipment.CurrentStatus.Should().Be(TransitNova.Domain.Enums.Shipment.ShipmentStatuses.Issue);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
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
        null,
        Guid.NewGuid(),
        PaymentMethod.PayPal
        );

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
        internal Mock<IUserQueryRepository> Users { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();
        internal Mock<ICacheService> Cache { get; } = new();
        internal CreateShipmentCommandHandler Handler { get; }

        internal CreateFixture()
        {
            Handler = new CreateShipmentCommandHandler(
                Service.Object,
                Users.Object,
                Logs.Object,
                UnitOfWork.Object,
                Mock.Of<ILogger<CreateShipmentCommandHandler>>());
        }
    }
}






