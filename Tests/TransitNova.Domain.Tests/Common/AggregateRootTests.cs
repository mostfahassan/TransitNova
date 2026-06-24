using FluentAssertions;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.Domain.Tests.Common;

public sealed class AggregateRootTests
{
    [Fact]
    public void RaiseDomainEvent_Should_Add_Event_When_Event_IsProvided()
    {
        var aggregate = new TestAggregate();
        var domainEvent = new TestDomainEvent(Guid.NewGuid());

        aggregate.RaiseDomainEvent(domainEvent);

        aggregate.GetDomainEvents().Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void ReleaseDomainEvent_Should_Remove_Only_Selected_Event_When_EventExists()
    {
        var aggregate = new TestAggregate();
        var retained = new TestDomainEvent(Guid.NewGuid());
        var released = new TestDomainEvent(Guid.NewGuid());
        aggregate.RaiseDomainEvent(retained);
        aggregate.RaiseDomainEvent(released);

        aggregate.ReleaseDomainEvent(released);

        aggregate.GetDomainEvents().Should().ContainSingle().Which.Should().Be(retained);
    }

    [Fact]
    public void ClearDomainEvents_Should_Remove_All_Events_When_EventsExist()
    {
        var aggregate = new TestAggregate();
        aggregate.RaiseDomainEvent(new TestDomainEvent(Guid.NewGuid()));
        aggregate.RaiseDomainEvent(new TestDomainEvent(Guid.NewGuid()));

        aggregate.ClearDomainEvents();

        aggregate.GetDomainEvents().Should().BeEmpty();
    }

    private sealed class TestAggregate : AggregateRoot<Guid>;

    private sealed record TestDomainEvent(Guid Id) : IDomainEvent;
}
