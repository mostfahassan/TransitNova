using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TransitNova.BusinessLayer.Common.Mappings.VehicleMapping;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Features.Vehicles.Handlers.ApplyQueries;
using TransitNova.BusinessLayer.Features.Vehicles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Handlers;

public sealed class VehicleHandlerTests
{
    [Fact]
    public async Task CreateVehicleHandler_Should_CreateTrimAndReturnVehicle_When_RequestIsValidAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        Vehicle? captured = null;
        repository.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Callback<Vehicle, CancellationToken>((vehicle, _) => captured = vehicle)
            .Returns(Task.CompletedTask);
        repository.Setup(x => x.GetByIdAsync<VehicleDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new VehicleDto { Id = id, PlateNumber = "ABC-123" });
        var handler = new CreateVehicleHandler(repository.Object, unitOfWork.Object, CreateMapper(),Mock.Of<ICacheService>(),Mock.Of<ILogger<CreateVehicleHandler>>());

        var result = await handler.Handle(
            new CreateVehicleCommand(Guid.NewGuid(), ValidCreateDto("  ABC-123  ")),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Created);
        captured.Should().NotBeNull();
        captured!.PlateNumber.Should().Be("ABC-123");
        repository.Verify(x => x.AddAsync(captured, CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateVehicleHandler_Should_ReturnFailure_When_CreatedVehicleCannotBeRetrievedAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<VehicleDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleDto?)null);
        var handler = new CreateVehicleHandler(repository.Object, unitOfWork.Object, CreateMapper(), Mock.Of<ICacheService>(), Mock.Of<ILogger<CreateVehicleHandler>>());

        var result = await handler.Handle(new CreateVehicleCommand(Guid.NewGuid(), ValidCreateDto()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Vehicle creation failed.");
    }

    [Fact]
    public async Task UpdateVehicleHandler_Should_UpdateVehicleAndCarrier_When_RequestIsValidAsync()
    {
        var oldCarrierId = Guid.NewGuid();
        var newCarrierId = Guid.NewGuid();
        var vehicle = Vehicle.Create(VehicleType.Van, "OLD", 100, 20, false, oldCarrierId);
        var repository = new Mock<IVehicleQueryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<Vehicle?>(vehicle.Id, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);
        var handler = new UpdateVehicleHandler(repository.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<UpdateVehicleHandler>>());
        var dto = ValidUpdateDto("  NEW-1  ");
        dto.CarrierId = newCarrierId;
        dto.IsRefrigerated = true;

        var result = await handler.Handle(new UpdateVehicleCommand(Guid.NewGuid(), vehicle.Id, dto), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        vehicle.PlateNumber.Should().Be("NEW-1");
        vehicle.CarrierId.Should().Be(newCarrierId);
        vehicle.IsRefrigerated.Should().BeTrue();
        repository.Verify(x => x.Update(vehicle), Times.Once);
    }

    [Fact]
    public async Task UpdateVehicleHandler_Should_ReturnNotFoundAndSkipCommit_When_VehicleDoesNotExistAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository.Setup(x => x.GetByIdAsync<Vehicle?>(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Vehicle?)null);
        var handler = new UpdateVehicleHandler(repository.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<UpdateVehicleHandler>>());

        var result = await handler.Handle(new UpdateVehicleCommand(Guid.NewGuid(), Guid.NewGuid(), ValidUpdateDto()), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteVehicleHandler_Should_DeleteAndCommit_When_CommandIsHandledAsync()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IVehicleQueryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<Vehicle>(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Vehicle.Create(VehicleType.Van, "ABC", 100, 20, false, Guid.NewGuid()));
        repository.Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new DeleteVehicleHandler(repository.Object, unitOfWork.Object, Mock.Of<ICacheService>(), Mock.Of<ILogger<DeleteVehicleHandler>>());

        var result = await handler.Handle(new DeleteVehicleCommand(Guid.NewGuid(), id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(id, CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetVehicleByIdHandler_Should_ReturnValidationFailure_When_IdIsEmptyAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var handler = new GetVehicleByIdHandler(repository.Object, Mock.Of<ILogger<GetVehicleByIdHandler>>());

        var result = await handler.Handle(new GetVehicleByIdQuery(Guid.Empty), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Failure);
        result.Error!.Message.Should().Be("Plate number is required.");
        repository.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false, ResultStatus.NotFound)]
    [InlineData(true, ResultStatus.Success)]
    public async Task GetVehicleByIdHandler_Should_ReturnExpectedResult_When_RepositoryResultIsKnownAsync(bool exists, ResultStatus expectedStatus)
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IVehicleQueryRepository>();
        repository.Setup(x => x.GetVehicleDetailsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists ? new VehicleDto { Id = id, PlateNumber = "ABC" } : null);
        var handler = new GetVehicleByIdHandler(repository.Object, Mock.Of<ILogger<GetVehicleByIdHandler>>());

        var result = await handler.Handle(new GetVehicleByIdQuery(id), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task GetVehicleByPlateNumberHandler_Should_ValidateBlankPlateWithoutRepositoryCallAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var handler = new GetVehicleByPlateNumberHandler(repository.Object, Mock.Of<ILogger<GetVehicleByPlateNumberHandler>>());

        var result = await handler.Handle(new GetVehicleByPlateNumberQuery("  "), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Plate number is required.");
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetVehicleByPlateNumberHandler_Should_TrimPlateAndReturnVehicle_When_FoundAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var dto = new VehicleDto { Id = Guid.NewGuid(), PlateNumber = "ABC" };
        repository.Setup(x => x.GetByPlateNumberAsync("ABC", It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        var handler = new GetVehicleByPlateNumberHandler(repository.Object, Mock.Of<ILogger<GetVehicleByPlateNumberHandler>>());

        var result = await handler.Handle(new GetVehicleByPlateNumberQuery("  ABC  "), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task GetVehicleListHandler_Should_ReturnEmptySuccess_When_NoVehiclesExistAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        repository.Setup(x => x.GetListAsync<VehicleDto>(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetVehicleListHandler(repository.Object, Mock.Of<ILogger<GetVehicleListHandler>>());

        var result = await handler.Handle(new GetVehicleListQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveVehiclesHandler_Should_ReturnRepositoryVehicles_When_ActiveVehiclesExistAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var vehicles = new List<VehicleDto> { new() { Id = Guid.NewGuid(), IsActive = true } };
        repository.Setup(x => x.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(vehicles);
        var handler = new GetActiveVehiclesHandler(repository.Object, Mock.Of<ILogger<GetActiveVehiclesHandler>>());

        var result = await handler.Handle(new GetActiveVehiclesQuery(), CancellationToken.None);

        result.Data.Should().BeSameAs(vehicles);
    }

    [Fact]
    public async Task GetVehiclesByCarrierIdHandler_Should_ReturnNotFoundWithoutRepositoryCall_When_CarrierIdIsEmptyAsync()
    {
        var repository = new Mock<IVehicleQueryRepository>();
        var handler = new GetVehiclesByCarrierIdHandler(repository.Object, Mock.Of<ILogger<GetVehiclesByCarrierIdHandler>>());

        var result = await handler.Handle(new GetVehiclesByCarrierIdQuery(Guid.Empty), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetVehiclesByCarrierIdHandler_Should_ReturnEmptySuccess_When_CarrierHasNoVehiclesAsync()
    {
        var carrierId = Guid.NewGuid();
        var repository = new Mock<IVehicleQueryRepository>();
        repository.Setup(x => x.GetByCarrierIdAsync(carrierId, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetVehiclesByCarrierIdHandler(repository.Object, Mock.Of<ILogger<GetVehiclesByCarrierIdHandler>>());

        var result = await handler.Handle(new GetVehiclesByCarrierIdQuery(carrierId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public void VehicleMappingProfile_Should_MapCreateVehicleDtoToVehicle_When_DataIsValid()
    {
        var dto = ValidCreateDto("  MAP-123  ");

        var vehicle = CreateMapper().Map<Vehicle>(dto);

        vehicle.Id.Should().NotBeEmpty();
        vehicle.VehicleType.Should().Be(dto.VehicleType);
        vehicle.PlateNumber.Should().Be("MAP-123");
        vehicle.CapacityWeight.Should().Be(dto.CapacityWeight);
        vehicle.CapacityVolume.Should().Be(dto.CapacityVolume);
        vehicle.IsRefrigerated.Should().Be(dto.IsRefrigerated);
        vehicle.CarrierId.Should().Be(dto.CarrierId);
        vehicle.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VehicleMappingProfile_Should_HaveValidConfiguration()
    {
        var configuration = CreateMapperConfiguration();

        Action action = configuration.AssertConfigurationIsValid;

        action.Should().NotThrow();
    }

    private static CreateVehicleDto ValidCreateDto(string plateNumber = "ABC-123") => new()
    {
        VehicleType = VehicleType.Van,
        PlateNumber = plateNumber,
        CapacityWeight = 1000,
        CapacityVolume = 50,
        CarrierId = Guid.NewGuid()
    };

    private static UpdateVehicleDto ValidUpdateDto(string plateNumber = "ABC-123") => new()
    {
        VehicleType = VehicleType.Van,
        PlateNumber = plateNumber,
        CapacityWeight = 1000,
        CapacityVolume = 50,
        CarrierId = Guid.NewGuid()
    };

    private static MapperConfiguration CreateMapperConfiguration() =>
        new(configuration => configuration.AddProfile<VehicleMappingProfile>(), NullLoggerFactory.Instance);

    private static IMapper CreateMapper() => CreateMapperConfiguration().CreateMapper();

    private static Mock<IUnitOfWork> SuccessfulUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return unitOfWork;
    }
}
