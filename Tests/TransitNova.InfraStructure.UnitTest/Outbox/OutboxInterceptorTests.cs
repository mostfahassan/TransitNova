using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Common.Interceptors;
using TransitNova.InfraStructure.OutBox;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Outbox;

public sealed class OutboxInterceptorTests
{
    [Fact]
    public async Task ConvertDomainEventsToOutboxMessages_DomainEvent_Should_CreateCompleteOutboxMessageAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync(
            new ConvertDomainEventsToOutboxMessages());
        var bundle = CreateBundleWithSubscription(out var userId);
        fixture.Context.Bundles.Add(bundle);

        await fixture.Context.SaveChangesAsync();

        var message = fixture.Context.ChangeTracker.Entries<OutboxMessage>()
            .Single().Entity;
        message.Id.Should().NotBeEmpty();
        message.OccuredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        message.ProcessedOn.Should().BeNull();
        message.Error.Should().BeNull();
        message.Type.Should().Be(typeof(UserSubscribedToBundleDomainEvent).AssemblyQualifiedName);

        var deserialized = JsonConvert.DeserializeObject<UserSubscribedToBundleDomainEvent>(message.Content!);
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(userId);
        deserialized.BundleId.Should().Be(bundle.Id);
    }

    [Fact]
    public async Task ConvertDomainEventsToOutboxMessages_FirstSave_Should_ClearAggregateEventsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync(
            new ConvertDomainEventsToOutboxMessages());
        var bundle = CreateBundleWithSubscription(out _);
        bundle.GetDomainEvents().Should().ContainSingle();
        fixture.Context.Bundles.Add(bundle);

        await fixture.Context.SaveChangesAsync();

        bundle.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public async Task ConvertDomainEventsToOutboxMessages_FirstSave_Should_PersistMessageInSameSaveAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync(
            new ConvertDomainEventsToOutboxMessages());
        fixture.Context.Bundles.Add(CreateBundleWithSubscription(out _));

        await fixture.Context.SaveChangesAsync();

        fixture.Context.ChangeTracker.Entries<OutboxMessage>()
            .Should().ContainSingle(x => x.State == EntityState.Unchanged);
        (await fixture.Context.OutboxMessages.AsNoTracking().CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ConvertDomainEventsToOutboxMessages_SecondSave_Should_PersistOneMessageWithoutDuplicationAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync(
            new ConvertDomainEventsToOutboxMessages());
        fixture.Context.Bundles.Add(CreateBundleWithSubscription(out _));
        await fixture.Context.SaveChangesAsync();

        await fixture.Context.SaveChangesAsync();

        (await fixture.Context.OutboxMessages.AsNoTracking().CountAsync()).Should().Be(1);
        fixture.Context.ChangeTracker.Entries<OutboxMessage>()
            .Should().ContainSingle(x => x.State == EntityState.Unchanged);
    }

    [Fact]
    public async Task ConvertDomainEventsToOutboxMessages_NoDomainEvents_Should_NotCreateMessageAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync(
            new ConvertDomainEventsToOutboxMessages());
        fixture.Context.Bundles.Add(Bundle.Create(
            "manager", "Business", 500m, "Description", 200m, 1_000m, 20));

        await fixture.Context.SaveChangesAsync();

        fixture.Context.ChangeTracker.Entries<OutboxMessage>().Should().BeEmpty();
        (await fixture.Context.OutboxMessages.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task ConvertDomainEventsToOutboxMessages_TwoDomainEvents_Should_CreateMessagesInOneBatchAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync(
            new ConvertDomainEventsToOutboxMessages());
        var first = CreateBundleWithSubscription(out _);
        var second = CreateBundleWithSubscription(out _);
        fixture.Context.Bundles.AddRange(first, second);

        await fixture.Context.SaveChangesAsync();

        fixture.Context.ChangeTracker.Entries<OutboxMessage>()
            .Should().HaveCount(2)
            .And.OnlyContain(x => x.State == EntityState.Unchanged);
    }

    private static Bundle CreateBundleWithSubscription(out Guid userId)
    {
        var bundle = Bundle.Create(
            "manager", $"Business-{Guid.NewGuid():N}", 500m, "Description", 200m, 1_000m, 20);
        typeof(Bundle).GetProperty(nameof(Bundle.Id))!.SetValue(bundle, Guid.NewGuid());
        userId = Guid.NewGuid();
        bundle.Subscribe(userId);
        return bundle;
    }
}
