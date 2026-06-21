using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.InfraStructure.Repository.Generic;
using TransitNova.InfraStructure.Repository.Idempotent;
using TransitNova.InfraStructure.Repository.SystemActivityLogs;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class RepositoryIntegrationTests
{
    [Fact]
    public async Task IdetmpoentRepository_CreateRequest_Should_PersistAllFields()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdetmpoentRepository(fixture.Context);
        var requestId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        await repository.CreateRequestAsync(requestId, "CreateShipmentCommand", CancellationToken.None);

        var stored = await fixture.Context.IdempotentTableKey.AsNoTracking().SingleAsync();
        stored.RequestId.Should().Be(requestId);
        stored.InstanceName.Should().Be("CreateShipmentCommand");
        stored.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IdetmpoentRepository_RequestExists_Should_ReturnExpectedResult(bool exists)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdetmpoentRepository(fixture.Context);
        var storedId = Guid.NewGuid();
        await repository.CreateRequestAsync(storedId, "Command", CancellationToken.None);

        var result = await repository.RequestExistsAsync(
            exists ? storedId : Guid.NewGuid(), CancellationToken.None);

        result.Should().Be(exists);
    }

    [Fact]
    public async Task IdetmpoentRepository_DuplicateRequestId_Should_ThrowRelationalConstraintException()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdetmpoentRepository(fixture.Context);
        var requestId = Guid.NewGuid();
        await repository.CreateRequestAsync(requestId, "FirstCommand", CancellationToken.None);
        fixture.Context.ChangeTracker.Clear();

        var act = () => repository.CreateRequestAsync(requestId, "SecondCommand", CancellationToken.None);

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task IdetmpoentRepository_CancelledToken_Should_ThrowWithoutPersisting()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdetmpoentRepository(fixture.Context);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var act = () => repository.CreateRequestAsync(
            Guid.NewGuid(), "Command", cancellation.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        (await fixture.Context.IdempotentTableKey.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task SystemLogCommands_Log_Should_TrackLogWithoutSavingImplicitly()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        var actorId = Guid.NewGuid();
        var log = SystemActivityLog.AddLog(
            ActivityAction.Created,
            ActivityEntityType.Shipment,
            "Shipment was created.",
            actorId,
            "Amina Hassan");

        await repository.Log(log, CancellationToken.None);

        fixture.Context.Entry(log).State.Should().Be(EntityState.Added);
        (await fixture.Context.SystemActivityLogs.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task SystemLogCommands_LogThenSave_Should_PersistActorAndDescription()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        var actorId = Guid.NewGuid();
        var log = SystemActivityLog.AddLog(
            ActivityAction.Updated,
            ActivityEntityType.Warehouse,
            "Warehouse Alexandria Hub was updated.",
            actorId,
            "Omar Fathy");
        await repository.Log(log, CancellationToken.None);

        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking().SingleAsync();
        stored.Action.Should().Be(ActivityAction.Updated);
        stored.EntityType.Should().Be(ActivityEntityType.Warehouse);
        stored.Description.Should().Be("Warehouse Alexandria Hub was updated.");
        stored.PerformedByUserId.Should().Be(actorId);
        stored.PerformedByName.Should().Be("Omar Fathy");
    }

    [Fact]
    public async Task SystemLogCommands_AnonymousActor_Should_PersistNullableUserId()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        var log = SystemActivityLog.AddLog(
            ActivityAction.Created,
            ActivityEntityType.User,
            "A user registered.",
            null,
            "New User");

        await repository.Log(log, CancellationToken.None);
        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking().SingleAsync();
        stored.PerformedByUserId.Should().BeNull();
        stored.PerformedByName.Should().Be("New User");
    }

    [Fact]
    public async Task SystemLogCommands_MultipleLogs_Should_PersistEachLogExactlyOnce()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        await repository.Log(SystemActivityLog.AddLog(
            ActivityAction.Created, ActivityEntityType.Trip, "Trip created.", Guid.NewGuid(), "Manager"),
            CancellationToken.None);
        await repository.Log(SystemActivityLog.AddLog(
            ActivityAction.Started, ActivityEntityType.Trip, "Trip started.", Guid.NewGuid(), "Carrier"),
            CancellationToken.None);

        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
        stored.Should().HaveCount(2);
        stored.Select(x => x.Action).Should().Equal(ActivityAction.Created, ActivityAction.Started);
    }

    [Fact]
    public async Task SystemLogCommands_Log_Should_PreserveFactoryOccurrenceTime()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        var log = SystemActivityLog.AddLog(
            ActivityAction.Delivered, ActivityEntityType.Shipment, "Shipment delivered.", Guid.NewGuid(), "Carrier");
        var occurredAt = log.OccurredAt;

        await repository.Log(log, CancellationToken.None);
        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking().SingleAsync();
        stored.OccurredAt.Should().Be(occurredAt);
    }

    [Fact]
    public async Task GenericRepository_AddAndGetList_Should_ProjectPersistedEntitiesWithoutTracking()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = CreateBundleRepository(fixture);
        var bundle = CreateBundle("Business");
        await repository.AddAsync(bundle, CancellationToken.None);
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();

        var result = await repository.GetListAsync<BundleProjection>(CancellationToken.None);

        result.Should().ContainSingle().Which.BundleName.Should().StartWith("Business-");
        fixture.Context.ChangeTracker.Entries<Bundle>().Should().BeEmpty();
    }

    [Fact]
    public async Task GenericRepository_GetById_Should_ReturnNullWhenEntityDoesNotExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = CreateBundleRepository(fixture);

        var result = await repository.GetByIdAsync<BundleProjection>(
            Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GenericRepository_CountWithPredicate_Should_CountMatchingRowsOnly()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = CreateBundleRepository(fixture);
        fixture.Context.Bundles.AddRange(
            CreateBundle("Active", currentState: true),
            CreateBundle("Inactive", currentState: false));
        await fixture.Context.SaveChangesAsync();

        var result = await repository.CountAsync(x => x.CurrentState, CancellationToken.None);

        result.Should().Be(1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GenericRepository_Delete_Should_ReturnExpectedResultAndRequireExplicitSave(bool exists)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = CreateBundleRepository(fixture);
        var id = Guid.NewGuid();
        if (exists)
        {
            var bundle = CreateBundle("Delete");
            typeof(Bundle).GetProperty(nameof(Bundle.Id))!.SetValue(bundle, id);
            fixture.Context.Bundles.Add(bundle);
            await fixture.Context.SaveChangesAsync();
            fixture.Context.ChangeTracker.Clear();
        }

        var result = await repository.DeleteAsync(id, CancellationToken.None);

        result.Should().Be(exists);
        (await fixture.Context.Bundles.AsNoTracking().AnyAsync(x => x.Id == id)).Should().Be(exists,
            "the repository does not own the unit-of-work commit");
        if (exists)
        {
            await fixture.Context.SaveChangesAsync();
            (await fixture.Context.Bundles.AsNoTracking().AnyAsync(x => x.Id == id)).Should().BeFalse();
        }
    }

    private static GenericRepository<Bundle, Guid> CreateBundleRepository(
        SqliteAppDbContextFixture fixture)
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.CreateMap<Bundle, BundleProjection>(),
            NullLoggerFactory.Instance);
        return new GenericRepository<Bundle, Guid>(fixture.Context, configuration);
    }

    private static Bundle CreateBundle(string name, bool currentState = true)
    {
        var bundle = Bundle.Create(
            "manager", $"{name}-{Guid.NewGuid():N}", 500m, "Description", 200m, 1_000m, 20);
        bundle.CurrentState = currentState;
        return bundle;
    }

    public sealed class BundleProjection
    {
        public Guid Id { get; init; }
        public string BundleName { get; init; } = string.Empty;
        public bool CurrentState { get; init; }
    }
}
