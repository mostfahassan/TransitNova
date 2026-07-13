using FluentAssertions;
using Microsoft.EntityFrameworkCore.Migrations;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Tests.Migrations;

public sealed class MigrationSmokeTests
{
    [Fact]
    public void InfrastructureAssembly_Should_ContainInitialMigrationMetadata()
    {
        var migrations = typeof(AppDbContext).Assembly
            .GetTypes()
            .Where(type => typeof(Migration).IsAssignableFrom(type))
            .Select(type => type.GetCustomAttributes(typeof(MigrationAttribute), inherit: false)
                .OfType<MigrationAttribute>()
                .SingleOrDefault()?.Id)
            .Where(id => id is not null)
            .ToList();

        migrations.Should().Contain("20260713151741_InitialMigration");
    }
}