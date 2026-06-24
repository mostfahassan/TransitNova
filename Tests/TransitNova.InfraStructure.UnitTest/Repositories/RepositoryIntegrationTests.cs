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
    public async Task IdempotentRepository_CreateRequest_Should_PersistAllFieldsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdempotentRepository(fixture.Context);
        var requestId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        await repository.CreateRequestAsync(requestId, "CreateShipmentCommand", "response", CancellationToken.None);

        var stored = await fixture.Context.IdempotentTableKey.AsNoTracking().SingleAsync();
        stored.RequestId.Should().Be(requestId);
        stored.InstanceName.Should().Be("CreateShipmentCommand");
        stored.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IdempotentRepository_RequestExists_Should_ReturnExpectedResultAsync(bool exists)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdempotentRepository(fixture.Context);
        var storedId = Guid.NewGuid();
        await repository.CreateRequestAsync(storedId, "Command", "response", CancellationToken.None);

        var result = await repository.RequestExistsAsync(
            exists ? storedId : Guid.NewGuid(), CancellationToken.None);

        result.Should().Be(exists);
    }

    [Fact]
    public async Task IdempotentRepository_DuplicateRequestId_Should_ThrowRelationalConstraintExceptionAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdempotentRepository(fixture.Context);
        var requestId = Guid.NewGuid();
        await repository.CreateRequestAsync(requestId, "FirstCommand", "response", CancellationToken.None);
        fixture.Context.ChangeTracker.Clear();

        var act = () => repository.CreateRequestAsync(requestId, "SecondCommand", "response", CancellationToken.None);

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task IdempotentRepository_CancelledToken_Should_ThrowWithoutPersistingAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new IdempotentRepository(fixture.Context);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var act = () => repository.CreateRequestAsync(
            Guid.NewGuid(), "Command","response" ,cancellation.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        (await fixture.Context.IdempotentTableKey.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task SystemLogCommands_Log_Should_TrackLogWithoutSavingImplicitlyAsync()
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

        await repository.LogAsync(log, CancellationToken.None);

        fixture.Context.Entry(log).State.Should().Be(EntityState.Added);
        (await fixture.Context.SystemActivityLogs.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task SystemLogCommands_LogThenSave_Should_PersistActorAndDescriptionAsync()
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
        await repository.LogAsync(log, CancellationToken.None);

        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking().SingleAsync();
        stored.Action.Should().Be(ActivityAction.Updated);
        stored.EntityType.Should().Be(ActivityEntityType.Warehouse);
        stored.Description.Should().Be("Warehouse Alexandria Hub was updated.");
        stored.PerformedByUserId.Should().Be(actorId);
        stored.PerformedByName.Should().Be("Omar Fathy");
    }

    [Fact]
    public async Task SystemLogCommands_AnonymousActor_Should_PersistNullableUserIdAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        var log = SystemActivityLog.AddLog(
            ActivityAction.Created,
            ActivityEntityType.User,
            "A user registered.",
            null,
            "New User");

        await repository.LogAsync(log, CancellationToken.None);
        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking().SingleAsync();
        stored.PerformedByUserId.Should().BeNull();
        stored.PerformedByName.Should().Be("New User");
    }

    [Fact]
    public async Task SystemLogCommands_MultipleLogs_Should_PersistEachLogExactlyOnceAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        await repository.LogAsync(SystemActivityLog.AddLog(
            ActivityAction.Created, ActivityEntityType.Trip, "Trip created.", Guid.NewGuid(), "Manager"),
            CancellationToken.None);
        await repository.LogAsync(SystemActivityLog.AddLog(
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
    public async Task SystemLogCommands_Log_Should_PreserveFactoryOccurrenceTimeAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = new SystemLogCommands(fixture.Context);
        var log = SystemActivityLog.AddLog(
            ActivityAction.Delivered, ActivityEntityType.Shipment, "Shipment delivered.", Guid.NewGuid(), "Carrier");
        var occurredAt = log.OccurredAt;

        await repository.LogAsync(log, CancellationToken.None);
        await fixture.Context.SaveChangesAsync();

        var stored = await fixture.Context.SystemActivityLogs.AsNoTracking().SingleAsync();
        stored.OccurredAt.Should().Be(occurredAt);
    }

    [Fact]
    public async Task GenericRepository_AddAndGetList_Should_ProjectPersistedEntitiesWithoutTrackingAsync()
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
    public async Task GenericRepository_GetById_Should_ReturnNullWhenEntityDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var repository = CreateBundleRepository(fixture);

        var result = await repository.GetByIdAsync<BundleProjection>(
            Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GenericRepository_CountWithPredicate_Should_CountMatchingRowsOnlyAsync()
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
    public async Task GenericRepository_Delete_Should_ReturnExpectedResultAndRequireExplicitSaveAsync(bool exists)
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
