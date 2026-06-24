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
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Commands.Carriers;

public sealed class CarrierCommandHandlerTests
{
    [Fact]
    public async Task UpdateCarrierStatusCommandHandler_WhenRepositoryUpdatesRow_ShouldReturnSuccessAndInvalidateCachesAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        var cache = new Mock<ICacheService>();
        var carrierId = Guid.NewGuid();
        repository.Setup(x => x.UpdateStatusAsync(carrierId, CarrierStatus.InActive, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new UpdateCarrierStatusCommandHandler(repository.Object, cache.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());

        var result = await handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), carrierId, CarrierStatus.InActive),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task UpdateCarrierStatusCommandHandler_WhenRepositoryUpdatesNoRows_ShouldReturnCarrierNotFoundWithoutCacheInvalidationAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        var cache = new Mock<ICacheService>();
        repository.Setup(x => x.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<CarrierStatus>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var handler = new UpdateCarrierStatusCommandHandler(repository.Object, cache.Object, Mock.Of<ILogger<UpdateCarrierStatusCommandHandler>>());

        var result = await handler.Handle(
            new UpdateCarrierStatusCommand(Guid.NewGuid(), Guid.NewGuid(), CarrierStatus.InActive),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Carrier Not Found");
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteCarrierHandler_WhenRepositoryDeletesRow_ShouldReturnSuccessAndInvalidateCachesAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        var cache = new Mock<ICacheService>();
        var carrierId = Guid.NewGuid();
        repository.Setup(x => x.DeleteCarrierAsync(carrierId, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new DeleteCarrierHandler(repository.Object, cache.Object, Mock.Of<ILogger<DeleteCarrierHandler>>());

        var result = await handler.Handle(
            new DeleteCarrierCommand(Guid.NewGuid(), carrierId, Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task DeleteCarrierHandler_WhenRepositoryDeletesNoRows_ShouldReturnFailureWithoutCacheInvalidationAsync()
    {
        var repository = new Mock<ICarrierCommandRepository>();
        var cache = new Mock<ICacheService>();
        repository.Setup(x => x.DeleteCarrierAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var handler = new DeleteCarrierHandler(repository.Object, cache.Object, Mock.Of<ILogger<DeleteCarrierHandler>>());

        var result = await handler.Handle(
            new DeleteCarrierCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        cache.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true, ResultStatus.Success)]
    [InlineData(false, ResultStatus.Conflict)]
    public async Task IsCarrierAvailableQueryHandler_WhenAvailabilityIsKnown_ShouldReturnExpectedResultAsync(bool available, ResultStatus expectedStatus)
    {
        var repository = new Mock<ICarrierRulesRepository>();
        var carrierId = Guid.NewGuid();
        repository.Setup(x => x.IsCarrierAvailableForAssignmentAsync(carrierId, It.IsAny<CancellationToken>())).ReturnsAsync(available);
        var handler = new IsCarrierAvailableQueryHandler(repository.Object, Mock.Of<ILogger<IsCarrierAvailableQueryHandler>>());

        var result = await handler.Handle(new CarrierAvailabilityCommand(Guid.NewGuid(), carrierId), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCarrierDoesNotExist_ShouldReturnNotFoundWithoutSavingAsync()
    {
        var repository = new Mock<ICarrierQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository.Setup(x => x.GetCarrierAsync(It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((Carrier?)null);
        var handler = new AddCarrierAdditionalInfoCommandHandler(repository.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<AddCarrierAdditionalInfoCommandHandler>>());

        var result = await handler.Handle(
            new AddingCarrierAdditionalInfoCommand(Guid.NewGuid(), ValidInfo(), Guid.NewGuid()),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenCarrierExists_ShouldUpdateSaveAndInvalidateCachesAsync()
    {
        var userId = Guid.NewGuid();
        var carrier = CreateCarrier(userId);
        var repository = new Mock<ICarrierQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        repository.Setup(x => x.GetCarrierAsync(It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(carrier);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new AddCarrierAdditionalInfoCommandHandler(repository.Object, unitOfWork.Object, cache.Object, Mock.Of<ILogger<AddCarrierAdditionalInfoCommandHandler>>());

        var result = await handler.Handle(
            new AddingCarrierAdditionalInfoCommand(Guid.NewGuid(), ValidInfo(), userId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        carrier.HasAdditionalInfo.Should().BeTrue();
        carrier.LicenseNumber.Should().Be("LIC-100");
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddCarrierAdditionalInfoCommandHandler_WhenAdditionalInfoAlreadyExists_ShouldPropagateDomainErrorWithoutSavingAsync()
    {
        var userId = Guid.NewGuid();
        var carrier = CreateCarrier(userId);
        carrier.AddAdditionalData(userId, "OLD", 5, 10, 2, DateTime.UtcNow, 1, Guid.NewGuid());
        var repository = new Mock<ICarrierQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository.Setup(x => x.GetCarrierAsync(It.IsAny<Expression<Func<Carrier, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(carrier);
        var handler = new AddCarrierAdditionalInfoCommandHandler(repository.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<AddCarrierAdditionalInfoCommandHandler>>());

        var act = () => handler.Handle(
            new AddingCarrierAdditionalInfoCommand(Guid.NewGuid(), ValidInfo(), userId),
            CancellationToken.None);

        await act.Should().ThrowAsync<DomainOperationException>().Where(x => x.ErrorCode == "CARRIER_ADDITIONAL_INFO_EXISTS");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Carrier CreateCarrier(Guid userId) =>
        Carrier.Create(userId, "Ahmed", "Ali", "carrier@example.com", "01000000000", "Cairo", 1);

    private static AdditionalInfoDto ValidInfo() => new()
    {
        LicenseNumber = "LIC-100",
        MaxDailyShipments = 10,
        DefaultCostPerKg = 15,
        YearsOfExperience = 3,
        ContractStartDate = DateTime.UtcNow.Date,
        ContractYears = 2,
        WarehouseId = Guid.NewGuid()
    };
}
