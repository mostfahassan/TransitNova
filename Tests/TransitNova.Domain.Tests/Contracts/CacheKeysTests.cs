using FluentAssertions;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.Domain.Tests.Contracts;

public sealed class CacheKeysTests
{
    [Fact]
    public void ShipmentByTrackingNumber_Should_EscapeUnsafeCharacters_When_TrackingNumberContainsSpaces()
    {
        var key = CacheKeys.Shipments.ByTrackingNumber("TRK 1/2");

        key.Should().Be("shipments:tracking-number:TRK%201%2F2");
    }

    [Fact]
    public void FilterCarriers_Should_CreateDeterministicKey_When_EquivalentObjectIsProvided()
    {
        var first = CacheKeys.Carriers.Filter(new { Page = 1, Search = "Cairo" });
        var second = CacheKeys.Carriers.Filter(new { Page = 1, Search = "Cairo" });

        first.Should().Be(second);
        first.Should().StartWith("carriers:filter:");
    }

    [Fact]
    public void OperationManagerHandledCarriers_Should_IncludeIdentityAndPagination_When_Called()
    {
        var managerId = Guid.NewGuid();

        var key = CacheKeys.OperationManagers.HandledCarriers(managerId, 2, 25);

        key.Should().Be($"operation-managers:handled-carriers:operation-manager-id:{managerId}:page-number:2:page-size:25");
    }

    [Fact]
    public void DefaultExpiration_Should_BeTwentyMinutes()
    {
        CacheKeys.DefaultExpiration.Should().Be(TimeSpan.FromMinutes(20));
    }
}
