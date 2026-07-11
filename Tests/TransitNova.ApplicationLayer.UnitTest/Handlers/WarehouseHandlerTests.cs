using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.DTOs.Warehouse;
using TransitNova.BusinessLayer.Features.Warehouses.Commands;
using TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Features.Warehouses.Handlers.ApplyQueries;
using TransitNova.BusinessLayer.Features.Warehouses.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.ApplicationLayer.Tests.Handlers;

public sealed class WarehouseHandlerTests
{
    [Fact]
    public async Task CreateWarehouseHandler_Should_ReturnValidationAndSkipWrites_When_AnyZoneIsMissingAsync()
    {
        var fixture = new Fixture();
        var zoneIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        fixture.Queries.Setup(x => x.GetZonesByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([Zone.Create("One", 1)]);

        var result = await fixture.CreateHandler.Handle(
            new CreateWarehouseCommand(Guid.NewGuid(), Guid.NewGuid(), CreateDto(zoneIds)),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.ValidationError);
        fixture.Commands.Verify(x => x.AddAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateWarehouseHandler_Should_DeduplicateZonesPersistLogAndReturnCreated_When_RequestIsValidAsync()
    {
        var fixture = new Fixture();
        var adminId = Guid.NewGuid();
        var zone = Zone.Create("Downtown",1);
        fixture.Queries.Setup(x => x.GetZonesByIdsAsync(
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(zone.Id)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([zone]);
        fixture.Admins.Setup(x => x.GetAdminNameAsync(adminId, It.IsAny<CancellationToken>())).ReturnsAsync("Sara Admin");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        Warehouse? warehouse = null;
        fixture.Commands.Setup(x => x.AddAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()))
            .Callback<Warehouse, CancellationToken>((value, _) => warehouse = value)
            .Returns(Task.CompletedTask);
        fixture.Queries.Setup(x => x.GetWarehouseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new WarehouseDto { Id = warehouse!.Id, Name = warehouse.Name });
        SystemActivityLog? activity = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((value, _) => activity = value)
            .Returns(Task.CompletedTask);

        var result = await fixture.CreateHandler.Handle(
            new CreateWarehouseCommand(Guid.NewGuid(), adminId, CreateDto([zone.Id, zone.Id])),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Created);
        warehouse!.ZonesServed.Should().ContainSingle().Which.Should().BeSameAs(zone);
        activity!.Action.Should().Be(ActivityAction.Created);
        activity.PerformedByName.Should().Be("Sara Admin");
        activity.PerformedByUserId.Should().Be(adminId);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateWarehouseHandler_Should_ReturnFailure_When_CreatedWarehouseCannotBeRetrievedAsync()
    {
        var fixture = new Fixture();
        fixture.Admins.Setup(x => x.GetAdminNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync("Admin");
        fixture.Queries.Setup(x => x.GetWarehouseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((WarehouseDto?)null);
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await fixture.CreateHandler.Handle(
            new CreateWarehouseCommand(Guid.NewGuid(), Guid.NewGuid(), CreateDto([])),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Warehouse not found");
    }

    [Fact]
    public async Task UpdateWarehouseHandler_Should_ReturnNotFoundAndSkipWrites_When_WarehouseDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.Queries.Setup(x => x.GetWarehouseForUpdateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Warehouse?)null);

        var result = await fixture.UpdateHandler.Handle(
            new UpdateWarehouseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), UpdateDto([])),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.Commands.Verify(x => x.Update(It.IsAny<Warehouse>()), Times.Never);
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateWarehouseHandler_Should_ReturnValidationAndSkipWrites_When_AnyZoneIsMissingAsync()
    {
        var fixture = new Fixture();
        var warehouse = Warehouse.Create("Old", WarehouseType.MainWarehouse, 100, 20, 8,Address.Create("Cairo", null, "Main Street"), Guid.NewGuid(), Guid.NewGuid());
        fixture.Queries.Setup(x => x.GetWarehouseForUpdateAsync(warehouse.Id, It.IsAny<CancellationToken>())).ReturnsAsync(warehouse);
        fixture.Queries.Setup(x => x.GetZonesByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await fixture.UpdateHandler.Handle(
            new UpdateWarehouseCommand(Guid.NewGuid(), warehouse.Id, Guid.NewGuid(), UpdateDto([Guid.NewGuid()])),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.ValidationError);
        fixture.Commands.Verify(x => x.Update(It.IsAny<Warehouse>()), Times.Never);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateWarehouseHandler_Should_UpdateZonesLogAndCommit_When_RequestIsValidAsync()
    {
        var fixture = new Fixture();
        var adminId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var warehouse = Warehouse.Create("Old", WarehouseType.MainWarehouse, 100, 20, 8, Address.Create("Old Address", null, "Main Street"), adminId, managerId);
        var zone = Zone.Create("North", 1);
        fixture.Queries.Setup(x => x.GetWarehouseForUpdateAsync(warehouse.Id, It.IsAny<CancellationToken>())).ReturnsAsync(warehouse);
        fixture.Queries.Setup(x => x.GetZonesByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync([zone]);
        fixture.Admins.Setup(x => x.GetAdminNameAsync(adminId, It.IsAny<CancellationToken>())).ReturnsAsync("Admin Name");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? activity = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((value, _) => activity = value)
            .Returns(Task.CompletedTask);

        var result = await fixture.UpdateHandler.Handle(
            new UpdateWarehouseCommand(Guid.NewGuid(), warehouse.Id, adminId, UpdateDto([zone.Id])),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        warehouse.Name.Should().Be("Updated Warehouse");
        warehouse.ZonesServed.Should().ContainSingle().Which.Should().BeSameAs(zone);
        activity!.Action.Should().Be(ActivityAction.Updated);
        activity.PerformedByName.Should().Be("Admin Name");
        fixture.Commands.Verify(x => x.Update(warehouse), Times.Once);
    }

    [Fact]
    public async Task DeleteWarehouseHandler_Should_ReturnNotFoundAndSkipLog_When_WarehouseDoesNotExistAsync()
    {
        var fixture = new Fixture();
        fixture.Commands.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await fixture.DeleteHandler.Handle(
            new DeleteWarehouseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        fixture.Admins.Verify(x => x.GetAdminNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        fixture.Logs.Verify(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteWarehouseHandler_Should_LogAndCommit_When_WarehouseExistsAsync()
    {
        var fixture = new Fixture();
        var warehouseId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        fixture.Commands.Setup(x => x.DeleteAsync(warehouseId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        fixture.Admins.Setup(x => x.GetAdminNameAsync(adminId, It.IsAny<CancellationToken>())).ReturnsAsync("Admin Name");
        fixture.UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        SystemActivityLog? activity = null;
        fixture.Logs.Setup(x => x.LogAsync(It.IsAny<SystemActivityLog>(), It.IsAny<CancellationToken>()))
            .Callback<SystemActivityLog, CancellationToken>((value, _) => activity = value)
            .Returns(Task.CompletedTask);

        var result = await fixture.DeleteHandler.Handle(
            new DeleteWarehouseCommand(Guid.NewGuid(), warehouseId, adminId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        activity!.Action.Should().Be(ActivityAction.Deleted);
        activity.PerformedByName.Should().Be("Admin Name");
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetWarehouseByIdHandler_Should_ReturnValidationFailure_When_IdIsEmptyAsync()
    {
        var queries = new Mock<IWarehouseQueriesRepository>();
        var handler = new GetWarehouseByIdHandler(queries.Object, Mock.Of<ILogger<GetWarehouseByIdHandler>>());

        var result = await handler.Handle(new GetWarehouseByIdQuery(Guid.Empty), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Warehouse id is required.");
        queries.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false, ResultStatus.NotFound)]
    [InlineData(true, ResultStatus.Success)]
    public async Task GetWarehouseByIdHandler_Should_ReturnExpectedResult_When_RepositoryResultIsKnownAsync(bool exists, ResultStatus expectedStatus)
    {
        var warehouseId = Guid.NewGuid();
        var queries = new Mock<IWarehouseQueriesRepository>();
        queries.Setup(x => x.GetWarehouseByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists ? new WarehouseDto { Id = warehouseId, Name = "Main" } : null);
        var handler = new GetWarehouseByIdHandler(queries.Object, Mock.Of<ILogger<GetWarehouseByIdHandler>>());

        var result = await handler.Handle(new GetWarehouseByIdQuery(warehouseId), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task GetWarehouseListHandler_Should_ReturnEmptySuccess_When_NoWarehousesExistAsync()
    {
        var queries = new Mock<IWarehouseQueriesRepository>();
        queries.Setup(x => x.GetWarehousesAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetWarehouseListHandler(queries.Object, Mock.Of<ILogger<GetWarehouseListHandler>>());

        var result = await handler.Handle(new GetWarehouseListQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    private static CreateWarehouseDto CreateDto(IReadOnlyCollection<Guid> zoneIds) => new()
    {
        Name = "Main Warehouse",
        Type = WarehouseType.MainWarehouse,
        Capacity = 1000,
        CurrentUsage = 100,
        OperatingHours = 12,
        Address =Address.Create("Cairo", null, "Main Street"),
        ZoneIds = zoneIds
    };

    private static UpdateWarehouseDto UpdateDto(IReadOnlyCollection<Guid> zoneIds) => new()
    {
        Name = "Updated Warehouse",
        Type = WarehouseType.BranchWarehouse,
        Capacity = 2000,
        CurrentUsage = 200,
        OperatingHours = 16,
        Address = Address.Create("Giza", null, "Main Street"),
        ZoneIds = zoneIds
    };

    private sealed class Fixture
    {
        internal Mock<IWarehouseCommandsRepository> Commands { get; } = new();
        internal Mock<IWarehouseQueriesRepository> Queries { get; } = new();
        internal Mock<IAdminQueryRepository> Admins { get; } = new();
        internal Mock<ISystemLogCommands> Logs { get; } = new();
        internal Mock<IUnitOfWork> UnitOfWork { get; } = new();

        internal CreateWarehouseHandler CreateHandler { get; }
        internal UpdateWarehouseHandler UpdateHandler { get; }
        internal DeleteWarehouseHandler DeleteHandler { get; }

        internal Fixture()
        {
            CreateHandler = new CreateWarehouseHandler(
                Commands.Object,
                Queries.Object,
                Admins.Object,
                Logs.Object,
                UnitOfWork.Object,
                Mock.Of<ILogger<CreateWarehouseHandler>>());
            UpdateHandler = new UpdateWarehouseHandler(
                Commands.Object,
                Queries.Object,
                Admins.Object,
                Logs.Object,
                UnitOfWork.Object,
                Mock.Of<ILogger<UpdateWarehouseHandler>>());
            DeleteHandler = new DeleteWarehouseHandler(
                Commands.Object,
                Admins.Object,
                Logs.Object,
                UnitOfWork.Object,
                Mock.Of<ILogger<DeleteWarehouseHandler>>());
        }
    }
}
