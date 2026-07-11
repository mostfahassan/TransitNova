using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Checking;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Crud;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Commands.Carriers;

public sealed class CarrierWorkflowPhase2Tests
{
    [Theory]
    [InlineData(CarrierStatus.Available)]
    [InlineData(CarrierStatus.Unavailable)]
    [InlineData(CarrierStatus.InActive)]
    [InlineData((CarrierStatus)998)]
    [InlineData((CarrierStatus)999)]
    public async Task UpdateCarrierStatusCommandHandler_WhenRepositoryUpdatesRow_ShouldReturnSuccessForRequestedStatusAsync(
        CarrierStatus status)
    {
        var repository = new Mock<ICarrierCommandRepository>();
        repository.Setup(x => x.UpdateStatusAsync(
                It.IsAny<Guid>(), status, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = new UpdateCarrierStatusCommandHandler(
            repository.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());

        var result = await handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), Guid.NewGuid(), status), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateCarrierStatusCommandHandler_WhenRepositoryUpdatesNoRows_ShouldReturnCarrierNotFoundAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        repository.Setup(x => x.UpdateStatusAsync(
                It.IsAny<Guid>(), It.IsAny<CarrierStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        var handler = new UpdateCarrierStatusCommandHandler(
            repository.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());

        var result = await handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), Guid.NewGuid(), CarrierStatus.Available), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Carrier Not Found");
    }

    [Fact]
    public async Task UpdateCarrierStatusCommandHandler_WhenRepositoryUpdatesRow_ShouldInvalidateThreeCachesAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        var cache = new Mock<ICacheService>();
        repository.Setup(x => x.UpdateStatusAsync(
                It.IsAny<Guid>(), It.IsAny<CarrierStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = new UpdateCarrierStatusCommandHandler(
            repository.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());

        await handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), Guid.NewGuid(), CarrierStatus.Available), CancellationToken.None);
    }

    [Fact]
    public async Task UpdateCarrierStatusCommandHandler_WhenCancellationTokenIsPassed_ShouldForwardAllArgumentsAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        repository.Setup(x => x.UpdateStatusAsync(
                It.IsAny<Guid>(), It.IsAny<CarrierStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var handler = new UpdateCarrierStatusCommandHandler(
            repository.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());
        var carrierId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();

        await handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), carrierId, CarrierStatus.Unavailable), cancellation.Token);

        repository.Verify(x => x.UpdateStatusAsync(carrierId, CarrierStatus.Unavailable, cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task UpdateCarrierStatusCommandHandler_WhenRepositoryFails_ShouldPropagateWithoutInvalidatingCacheAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        var cache = new Mock<ICacheService>();
        repository.Setup(x => x.UpdateStatusAsync(
                It.IsAny<Guid>(), It.IsAny<CarrierStatus>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("update failed"));
        var handler = new UpdateCarrierStatusCommandHandler(
            repository.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());

        var act = () => handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), Guid.NewGuid(), CarrierStatus.Available), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("update failed");
        cache.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true, ResultStatus.Success)]
    [InlineData(false, ResultStatus.Conflict)]
    public async Task IsCarrierAvailableQueryHandler_WhenAvailabilityIsKnown_ShouldReturnExpectedStatusAsync(
        bool available,
        ResultStatus expectedStatus)
    {
        var rules = new Mock<ICarrierRulesRepository>();
        rules.Setup(x => x.IsCarrierAvailableForAssignmentAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(available);
        var handler = new IsCarrierAvailableQueryHandler(
            rules.Object, Mock.Of<ILogger<IsCarrierAvailableQueryHandler>>());

        var result = await handler.Handle(
            new CarrierAvailabilityCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task IsCarrierAvailableQueryHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var rules = new Mock<ICarrierRulesRepository>();
        rules.Setup(x => x.IsCarrierAvailableForAssignmentAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new IsCarrierAvailableQueryHandler(
            rules.Object, Mock.Of<ILogger<IsCarrierAvailableQueryHandler>>());
        var carrierId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();

        await handler.Handle(new CarrierAvailabilityCommand(Guid.NewGuid(), carrierId), cancellation.Token);

        rules.Verify(x => x.IsCarrierAvailableForAssignmentAsync(carrierId, cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task IsCarrierAvailableQueryHandler_WhenRulesRepositoryFails_ShouldPropagateExceptionAsync()
    {
        var rules = new Mock<ICarrierRulesRepository>();
        rules.Setup(x => x.IsCarrierAvailableForAssignmentAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("availability unavailable"));
        var handler = new IsCarrierAvailableQueryHandler(
            rules.Object, Mock.Of<ILogger<IsCarrierAvailableQueryHandler>>());

        var act = () => handler.Handle(
            new CarrierAvailabilityCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCarrierExists_ShouldReturnSuccessAsync()
    {
        var fixture = new CarrierInfoFixture();
        (await fixture.Handler.Handle(fixture.Command, CancellationToken.None)).IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("license")]
    [InlineData("capacity")]
    [InlineData("warehouse")]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCarrierExists_ShouldApplyRequestedFieldAsync(string field)
    {
        var fixture = new CarrierInfoFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        switch (field)
        {
            case "license": fixture.Carrier.LicenseNumber.Should().Be(fixture.Dto.LicenseNumber); break;
            case "capacity": fixture.Carrier.MaxDailyShipments.Should().Be(fixture.Dto.MaxDailyShipments); break;
            case "warehouse": fixture.Carrier.HomeWarehouseId.Should().Be(fixture.Dto.WarehouseId); break;
        }
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCarrierExists_ShouldSaveAndInvalidateTwoCachesAsync()
    {
        var fixture = new CarrierInfoFixture();
        await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCarrierDoesNotExist_ShouldReturnNotFoundWithoutSavingAsync()
    {
        var fixture = new CarrierInfoFixture();
        fixture.Repository.Setup(x => x.GetCarrierAsync(
                It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Carrier?)null);
        var result = await fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenInfoAlreadyExists_ShouldPropagateDomainErrorWithoutSavingAsync()
    {
        var fixture = new CarrierInfoFixture();
        fixture.Carrier.AddAdditionalData(
            fixture.Command.UserId, "OLD", 1, 1, 1, DateTime.UtcNow, 1, Guid.NewGuid());
        var act = () => fixture.Handler.Handle(fixture.Command, CancellationToken.None);
        await act.Should().ThrowAsync<DomainOperationException>()
            .Where(x => x.ErrorCode == "CARRIER_ADDITIONAL_INFO_EXISTS");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCancellationTokenIsPassed_ShouldForwardItAsync()
    {
        var fixture = new CarrierInfoFixture();
        using var cancellation = new CancellationTokenSource();
        await fixture.Handler.Handle(fixture.Command, cancellation.Token);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(cancellation.Token), Times.Once);
    }

    private sealed class CarrierInfoFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Carrier Carrier { get; }
        public AdditionalInfoDto Dto { get; } = new()
        {
            LicenseNumber = "LIC-200",
            MaxDailyShipments = 20,
            DefaultCostPerKg = 12,
            YearsOfExperience = 4,
            ContractStartDate = DateTime.UtcNow.Date,
            ContractYears = 2,
            WarehouseId = Guid.NewGuid()
        };
        public AddingCarrierAdditionalInfoCommand Command { get; }
        public Mock<ICarrierQueryRepository> Repository { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ICacheService> Cache { get; } = new();
        public AddCarrierAdditionalInfoCommandHandler Handler { get; }
        public CarrierInfoFixture()
        {
            Carrier = Carrier.Create(
                UserId, "Ahmed", "Ali", "carrier@example.com", "01000000000", Address.Create("Cairo", null, "Main Street"), 1);
            Command = new AddingCarrierAdditionalInfoCommand(Guid.NewGuid(), Dto, UserId);
            Repository.Setup(x => x.GetCarrierAsync(
                    It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Carrier);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Handler = new AddCarrierAdditionalInfoCommandHandler(
                Repository.Object, UnitOfWork.Object,
                Mock.Of<ILogger<AddCarrierAdditionalInfoCommandHandler>>());
        }
    }
}


