using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.EntityConfigurations;

public sealed class EntityConfigurationTests
{
    [Fact]
    public async Task CountryConfiguration_NameAndGovernmentRelationship_Should_EnforceConfiguredMetadataAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<Country>(fixture);

        entity.FindPrimaryKey()!.Properties.Should().ContainSingle(x => x.Name == nameof(Country.Id));
        var name = entity.FindProperty(nameof(Country.Name))!;
        name.IsNullable.Should().BeFalse();
        name.GetMaxLength().Should().Be(100);
        entity.GetReferencingForeignKeys().Single(x => x.DeclaringEntityType.ClrType == typeof(Government))
            .DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public async Task CityConfiguration_GovernmentAndNameIndex_Should_BeCompositeAndUniqueAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var index = FindIndex(Entity<City>(fixture), nameof(City.GovernmentId), nameof(City.Name));

        index.Should().NotBeNull();
        index!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public async Task ZoneConfiguration_CityRelationshipAndWarehouseNavigation_Should_BeConfiguredAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<Zone>(fixture);

        entity.GetForeignKeys().Single(x => x.Properties.Single().Name == nameof(Zone.CityId))
            .DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
        entity.GetSkipNavigations().Should().ContainSingle(x => x.Name == nameof(Zone.Warehouses));
        FindIndex(entity, nameof(Zone.CityId)).Should().NotBeNull();
        FindIndex(entity, nameof(Zone.Name)).Should().NotBeNull();
    }

    [Fact]
    public async Task VehicleConfiguration_PlateCarrierAndType_Should_EnforceConstraintsAndConversionsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<Vehicle>(fixture);

        entity.FindProperty(nameof(Vehicle.PlateNumber))!.GetMaxLength().Should().Be(20);
        FindIndex(entity, nameof(Vehicle.PlateNumber))!.IsUnique.Should().BeTrue();
        entity.FindProperty(nameof(Vehicle.VehicleType))!.GetProviderClrType().Should().Be(typeof(string));
        var carrierForeignKey = entity.GetForeignKeys()
            .Single(x => x.Properties.Single().Name == nameof(Vehicle.CarrierId));
        carrierForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
        carrierForeignKey.IsUnique.Should().BeTrue();
        
    }

    [Fact]
    public async Task RefreshTokenConfiguration_TokenAndActiveFilter_Should_EnforceUniquenessAndFilteringAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<RefreshToken>(fixture);

        entity.FindProperty(nameof(RefreshToken.Token))!.GetMaxLength().Should().Be(150);
        FindIndex(entity, nameof(RefreshToken.Token))!.IsUnique.Should().BeTrue();
      
        entity.GetForeignKeys().Single(x => x.Properties.Single().Name == nameof(RefreshToken.UserId))
            .DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
    }

    [Fact]
    public async Task SystemActivityLogConfiguration_DescriptionActorAndIndexes_Should_MatchAuditContractAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<SystemActivityLog>(fixture);

        entity.GetTableName().Should().Be("SystemActivityLogs");
        entity.FindProperty(nameof(SystemActivityLog.Description))!.GetMaxLength().Should().Be(500);
        entity.FindProperty(nameof(SystemActivityLog.PerformedByName))!.GetMaxLength().Should().Be(150);
        FindIndex(entity, nameof(SystemActivityLog.PerformedByName), nameof(SystemActivityLog.OccurredAt))
            .Should().NotBeNull();
        FindIndex(entity, nameof(SystemActivityLog.PerformedByUserId)).Should().NotBeNull();
    }

    [Fact]
    public async Task IdempotentTableConfiguration_RequestIdAndInstanceName_Should_EnforceIdempotencyShapeAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<IdempotentTable>(fixture);

        entity.FindPrimaryKey()!.Properties.Should().ContainSingle(x => x.Name == nameof(IdempotentTable.RequestId));
        var instanceName = entity.FindProperty(nameof(IdempotentTable.InstanceName))!;
        instanceName.IsNullable.Should().BeFalse();
        instanceName.GetMaxLength().Should().Be(30);
        FindIndex(entity, nameof(IdempotentTable.InstanceName), nameof(IdempotentTable.CreatedAt))
            .Should().NotBeNull();
    }

    [Fact]
    public async Task ShipmentConfiguration_OwnedSpecificationAndOperationalIndexes_Should_BeConfiguredAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<Shipment>(fixture);

        entity.GetDeclaredQueryFilters().Should().NotBeEmpty();
        FindIndex(entity, nameof(Shipment.TrackingNumber))!.IsUnique.Should().BeTrue();
        FindIndex(entity, nameof(Shipment.CurrentStatus), nameof(Shipment.PickupDate), nameof(Shipment.CreatedAt))
            .Should().NotBeNull();
        fixture.Context.Model.GetEntityTypes()
            .Should().Contain(x => x.IsOwned() && x.ClrType.Name.Contains("PackageSpecification"));
    }

    [Fact]
    public async Task BundleConfiguration_NameAndOperationalIndexes_Should_EnforceConfiguredMetadataAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var entity = Entity<Bundle>(fixture);

        entity.FindProperty(nameof(Bundle.BundleName))!.GetMaxLength().Should().Be(50);
        FindIndex(entity, nameof(Bundle.BundleName))!.IsUnique.Should().BeTrue();
        FindIndex(entity, nameof(Bundle.CurrentState), nameof(Bundle.CreatedAt)).Should().NotBeNull();
        FindIndex(entity, nameof(Bundle.BundlePrice), nameof(Bundle.CurrentState)).Should().NotBeNull();
    }

    private static IEntityType Entity<TEntity>(SqliteAppDbContextFixture fixture) =>
        fixture.Context.Model.FindEntityType(typeof(TEntity))
        ?? throw new InvalidOperationException($"{typeof(TEntity).Name} is missing from the EF model.");

    private static IIndex? FindIndex(IEntityType entity, params string[] propertyNames) =>
        entity.GetIndexes().SingleOrDefault(index =>
            index.Properties.Select(property => property.Name).SequenceEqual(propertyNames));
}
