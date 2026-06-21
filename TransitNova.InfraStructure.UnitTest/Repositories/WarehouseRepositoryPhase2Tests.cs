using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransitNova.InfraStructure.Repository.WarehouseRepo;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class WarehouseRepositoryPhase2Tests
{
    [Fact]
    public async Task GetWarehousesAsync_ShouldReturnEmptyList_WhenNoWarehousesExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new WarehouseRepository(fixture.Context).GetWarehousesAsync(default)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetWarehousesAsync_ShouldOrderByName_WhenRowsExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await Phase2RepositoryTestData.AddWarehouseAsync(fixture, "Zulu Hub");
        await Phase2RepositoryTestData.AddWarehouseAsync(fixture, "Alpha Hub");
        var result = await new WarehouseRepository(fixture.Context).GetWarehousesAsync(default);
        result.Select(x => x.Name).Should().Equal("Alpha Hub", "Zulu Hub");
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_ShouldReturnProjection_WhenWarehouseExists()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var warehouse = await Phase2RepositoryTestData.AddWarehouseAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await new WarehouseRepository(fixture.Context).GetWarehouseByIdAsync(warehouse.Id, default);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Central Hub");
        fixture.Context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetWarehouseByIdAsync_ShouldReturnNull_WhenWarehouseDoesNotExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new WarehouseRepository(fixture.Context).GetWarehouseByIdAsync(Guid.NewGuid(), default)).Should().BeNull();
    }

    [Fact]
    public async Task GetWarehouseForUpdateAsync_ShouldTrackWarehouseAndIncludeZones_WhenWarehouseExists()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var zone = await Phase2RepositoryTestData.AddZoneAsync(fixture, location.City);
        var warehouse = await Phase2RepositoryTestData.AddWarehouseAsync(fixture, zone: zone);
        fixture.Context.ChangeTracker.Clear();
        var result = await new WarehouseRepository(fixture.Context).GetWarehouseForUpdateAsync(warehouse.Id, default);
        result!.ZonesServed.Should().ContainSingle(x => x.Id == zone.Id);
        fixture.Context.Entry(result).State.Should().Be(EntityState.Unchanged);
    }

    [Fact]
    public async Task GetZonesByIdsAsync_ShouldReturnDistinctMatchingZones_WhenIdsContainDuplicates()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var zone = await Phase2RepositoryTestData.AddZoneAsync(fixture, location.City);
        var result = await new WarehouseRepository(fixture.Context)
            .GetZonesByIdsAsync([zone.Id, zone.Id, Guid.NewGuid()], default);
        result.Should().ContainSingle(x => x.Id == zone.Id);
    }

    [Fact]
    public async Task GetZonesByIdsAsync_ShouldReturnEmptyList_WhenNoIdsMatch()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var result = await new WarehouseRepository(fixture.Context)
            .GetZonesByIdsAsync([Guid.NewGuid()], default);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWarehouseGovernmentAsync_ShouldReturnWarehouseId_WhenZoneBelongsToGovernment()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var zone = await Phase2RepositoryTestData.AddZoneAsync(fixture, location.City);
        var warehouse = await Phase2RepositoryTestData.AddWarehouseAsync(fixture, zone: zone);
        var result = await new WarehouseRepository(fixture.Context)
            .GetWarehouseGovernmentAsync(location.Government.Id, default);
        result.Should().Be(warehouse.Id);
    }

    [Fact]
    public async Task GetWarehouseGovernmentAsync_ShouldReturnNull_WhenGovernmentHasNoWarehouse()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new WarehouseRepository(fixture.Context).GetWarehouseGovernmentAsync(999, default)).Should().BeNull();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldMatchTrimmedNameIgnoringCase_WhenWarehouseExists()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await Phase2RepositoryTestData.AddWarehouseAsync(fixture, "Central Hub");
        var repository = new WarehouseRulesRepository(fixture.Context, Phase2RepositoryTestData.CreateMapper());
        (await repository.ExistsByNameAsync("  CENTRAL HUB ", null, default)).Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldIgnoreExcludedWarehouse_WhenIdsMatch()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var warehouse = await Phase2RepositoryTestData.AddWarehouseAsync(fixture, "Central Hub");
        var repository = new WarehouseRulesRepository(fixture.Context, Phase2RepositoryTestData.CreateMapper());
        (await repository.ExistsByNameAsync("Central Hub", warehouse.Id, default)).Should().BeFalse();
    }
}
