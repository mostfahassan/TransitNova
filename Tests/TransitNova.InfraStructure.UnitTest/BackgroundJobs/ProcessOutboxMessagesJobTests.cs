using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Quartz;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents;
using TransitNova.InfraStructure.BackgroundJobs;
using TransitNova.InfraStructure.OutBox;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.BackgroundJobs;

public sealed class ProcessOutboxMessagesJobTests
{
    [Fact]
    public async Task ProcessOutboxMessagesJob_ValidMessage_Should_PublishAndMarkProcessedAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        publisher.Setup(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var message = CreateMessage(DateTime.UtcNow);
        fixture.Context.OutboxMessages.Add(message);
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        publisher.Verify(x => x.Publish(
            It.Is<IDomainEvent>(e => e.GetType() == typeof(UserSubscribedToBundleDomainEvent) &&
                ((UserSubscribedToBundleDomainEvent)e).Id != Guid.Empty &&
                ((UserSubscribedToBundleDomainEvent)e).BundleId != Guid.Empty),
            CancellationToken.None), Times.Once);
        var stored = await fixture.Context.OutboxMessages.AsNoTracking().SingleAsync();
        stored.ProcessedOn.Should().NotBeNull();
        stored.ProcessedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_ProcessedMessage_Should_SkipPublishingAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        var message = CreateMessage(DateTime.UtcNow);
        message.ProcessedOn = DateTime.UtcNow.AddMinutes(-1);
        fixture.Context.OutboxMessages.Add(message);
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        publisher.Verify(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_TwentyFiveMessages_Should_ProcessOldestTwentyOnlyAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var published = new List<Guid>();
        var publisher = new Mock<IPublisher>();
        publisher.Setup(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Callback<IDomainEvent, CancellationToken>((domainEvent, _) =>
                published.Add(((UserSubscribedToBundleDomainEvent)domainEvent).Id))
            .Returns(Task.CompletedTask);
        var now = DateTime.UtcNow;
        var messages = Enumerable.Range(0, 25)
            .Select(index => CreateMessage(now.AddMinutes(index)))
            .ToList();
        fixture.Context.OutboxMessages.AddRange(messages);
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        published.Should().HaveCount(20);
        published.Should().Equal(messages.Take(20).Select(ReadEventId));
        var unprocessed = await fixture.Context.OutboxMessages.AsNoTracking()
            .Where(x => x.ProcessedOn == null)
            .OrderBy(x => x.OccuredAt)
            .ToListAsync();
        unprocessed.Select(x => x.Id).Should().Equal(messages.Skip(20).Select(x => x.Id));
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_UnknownEventType_Should_RecordFailureWithoutPublishingAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        var message = CreateMessage(DateTime.UtcNow);
        message.Type = "TransitNova.Does.Not.Exist, TransitNova.Domain";
        fixture.Context.OutboxMessages.Add(message);
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        publisher.Verify(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        var stored = await fixture.Context.OutboxMessages.AsNoTracking().SingleAsync();
        stored.ProcessedOn.Should().BeNull();
        stored.RetryCount.Should().Be(1);
        stored.Error.Should().StartWith("System.InvalidOperationException");
        stored.Error.Should().Contain($"Unable to resolve event type '{message.Type}'.");
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_InvalidJson_Should_RecordFailureWithoutPublishingAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        var message = CreateMessage(DateTime.UtcNow);
        message.Content = "{ invalid-json";
        fixture.Context.OutboxMessages.Add(message);
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        publisher.Verify(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        var stored = await fixture.Context.OutboxMessages.AsNoTracking().SingleAsync();
        stored.ProcessedOn.Should().BeNull();
        stored.RetryCount.Should().Be(1);
        stored.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_PublisherFailure_Should_RecordFailureAndLeaveMessageUnprocessedAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        publisher.Setup(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("broker unavailable"));
        fixture.Context.OutboxMessages.Add(CreateMessage(DateTime.UtcNow));
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        var stored = await fixture.Context.OutboxMessages.AsNoTracking().SingleAsync();
        stored.ProcessedOn.Should().BeNull();
        stored.RetryCount.Should().Be(1);
        stored.Error.Should().StartWith("System.InvalidOperationException");
        stored.Error.Should().Contain("broker unavailable");
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_SecondPublishFails_Should_PersistFirstSuccessAndRecordSecondFailureAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        publisher.SetupSequence(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new InvalidOperationException("second publish failed"));
        fixture.Context.OutboxMessages.AddRange(
            CreateMessage(DateTime.UtcNow.AddMinutes(-2)),
            CreateMessage(DateTime.UtcNow.AddMinutes(-1)));
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        var stored = await fixture.Context.OutboxMessages.AsNoTracking()
            .OrderBy(x => x.OccuredAt)
            .ToListAsync();
        stored[0].ProcessedOn.Should().NotBeNull();
        stored[0].RetryCount.Should().Be(0);
        stored[1].ProcessedOn.Should().BeNull();
        stored[1].RetryCount.Should().Be(1);
        stored[1].Error.Should().StartWith("System.InvalidOperationException");
        stored[1].Error.Should().Contain("second publish failed");
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_MessageAtRetryLimit_Should_SkipPublishingAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        var message = CreateMessage(DateTime.UtcNow);
        message.RetryCount = 5;
        fixture.Context.OutboxMessages.Add(message);
        await fixture.Context.SaveChangesAsync();

        await CreateJob(fixture, publisher).Execute(CreateExecutionContext());

        publisher.Verify(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        var stored = await fixture.Context.OutboxMessages.AsNoTracking().SingleAsync();
        stored.ProcessedOn.Should().BeNull();
        stored.RetryCount.Should().Be(5);
    }

    [Fact]
    public async Task ProcessOutboxMessagesJob_CancelledToken_Should_ThrowWithoutPublishingAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var publisher = new Mock<IPublisher>();
        fixture.Context.OutboxMessages.Add(CreateMessage(DateTime.UtcNow));
        await fixture.Context.SaveChangesAsync();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var act = () => CreateJob(fixture, publisher)
            .Execute(CreateExecutionContext(cancellation.Token));

        await act.Should().ThrowAsync<OperationCanceledException>();
        publisher.Verify(x => x.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static ProcessOutboxMessagesJob CreateJob(
       SqliteAppDbContextFixture fixture,
       Mock<IPublisher> publisher,
       Mock<ILogger<ProcessOutboxMessagesJob>>? logger = null)
    {
        logger ??= new Mock<ILogger<ProcessOutboxMessagesJob>>();

        return new ProcessOutboxMessagesJob(
            fixture.Context,
            publisher.Object,
            logger.Object);
    }
    private static IJobExecutionContext CreateExecutionContext(CancellationToken cancellationToken = default)
    {
        var context = new Mock<IJobExecutionContext>();
        context.SetupGet(x => x.CancellationToken).Returns(cancellationToken);
        return context.Object;
    }

    private static OutboxMessage CreateMessage(DateTime occurredAt)
    {
        var domainEvent = new UserSubscribedToBundleDomainEvent(Guid.NewGuid(), Guid.NewGuid());
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccuredAt = occurredAt,
            Type = typeof(UserSubscribedToBundleDomainEvent).AssemblyQualifiedName,
            Content = JsonConvert.SerializeObject(domainEvent)
        };
    }

    private static Guid ReadEventId(OutboxMessage message) =>
        JsonConvert.DeserializeObject<UserSubscribedToBundleDomainEvent>(message.Content!)!.Id;
}
