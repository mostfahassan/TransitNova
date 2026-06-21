using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.RegularExpressions;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.InfraStructure.Tests.TestInfrastructure;

internal static class Phase2RepositoryTestData
{
    internal static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(RetrieveShipmentDto).Assembly),
            NullLoggerFactory.Instance);
        return configuration.CreateMapper();
    }

    internal static async Task<(Country Country, Government Government, City City)> AddLocationAsync(
        SqliteAppDbContextFixture fixture, string suffix = "One")
    {
        var country = Country.Create($"Egypt {suffix}");
        fixture.Context.Countries.Add(country);
        await fixture.Context.SaveChangesAsync();

        var government = Government.Create($"Cairo {suffix}", country.Id);
        fixture.Context.Governments.Add(government);
        await fixture.Context.SaveChangesAsync();

        var city = City.Create($"Nasr City {suffix}", government.Id);
        fixture.Context.Cities.Add(city);
        await fixture.Context.SaveChangesAsync();
        return (country, government, city);
    }

    internal static async Task<Zone> AddZoneAsync(
        SqliteAppDbContextFixture fixture, City city, string suffix = "One")
    {
        var zone = Zone.Create($"Zone {suffix}", $"Z-{suffix}", city.Id);
        fixture.Context.Zones.Add(zone);
        await fixture.Context.SaveChangesAsync();
        return zone;
    }

    internal static async Task<Warehouse> AddWarehouseAsync(
        SqliteAppDbContextFixture fixture, string name = "Central Hub", Zone? zone = null)
    {
        await EnsureSqliteRowVersionDefaultAsync(fixture, "Warehouses");
        var warehouse = Warehouse.Create(
            name, WarehouseType.MainWarehouse, 1_000m, 100m, 24, "Cairo", Guid.NewGuid());
        typeof(Warehouse).GetProperty(nameof(Warehouse.RowVersion))!.SetValue(warehouse, new byte[] { 1 });
        if (zone is not null)
            warehouse.AddZone(zone);
        fixture.Context.Warehouses.Add(warehouse);
        await fixture.Context.SaveChangesAsync();
        return warehouse;
    }

    internal static async Task<(AppUser AppUser, UserProfile Profile)> AddUserAsync(
        SqliteAppDbContextFixture fixture, City city, string suffix = "One")
    {
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = $"user-{suffix}",
            NormalizedUserName = $"USER-{suffix}".ToUpperInvariant(),
            Email = $"user-{suffix}@example.com",
            NormalizedEmail = $"USER-{suffix}@EXAMPLE.COM".ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            UserType = UserType.User
        };
        fixture.Context.AppUsers.Add(appUser);
        await fixture.Context.SaveChangesAsync();

        var profile = UserProfile.Create(
            appUser.Id, "Amina", suffix, appUser.Email!, "01000000000", "Cairo", city.Id);
        fixture.Context.UserProfiles.Add(profile);
        await fixture.Context.SaveChangesAsync();
        return (appUser, profile);
    }

    internal static async Task<Carrier> AddCarrierAsync(
        SqliteAppDbContextFixture fixture, City city, string suffix = "One")
    {
        await EnsureSqliteRowVersionDefaultAsync(fixture, "Carriers");
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = $"carrier-{suffix}",
            NormalizedUserName = $"CARRIER-{suffix}".ToUpperInvariant(),
            Email = $"carrier-{suffix}@example.com",
            NormalizedEmail = $"CARRIER-{suffix}@EXAMPLE.COM".ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            UserType = UserType.Carrier
        };
        fixture.Context.AppUsers.Add(appUser);
        await fixture.Context.SaveChangesAsync();
        var carrier = Carrier.Create(
            appUser.Id, "Omar", suffix, appUser.Email!, "01100000000", "Cairo", city.Id);
        carrier.AddAdditionalData(
            appUser.Id, $"LIC-{suffix}", 20, 12.5m, 5, DateTime.UtcNow.Date, 1, null);
        typeof(Carrier).GetProperty(nameof(Carrier.RowVersion))!.SetValue(carrier, new byte[] { 1 });
        fixture.Context.Carriers.Add(carrier);
        await fixture.Context.SaveChangesAsync();
        return carrier;
    }

    internal static async Task<Shipment> AddShipmentAsync(
        SqliteAppDbContextFixture fixture, UserProfile sender, City city)
    {
        await EnsureSqliteRowVersionDefaultAsync(fixture, "Shipments");
        var receiver = ReceiverProfile.Create(
            "Mona", "Ali", "mona@example.com", "01200000000", "Giza", city.Id, sender.Id);
        receiver.Sender = sender;
        var shipment = Shipment.Create(
            sender.Id,
            receiver,
            new PackageSpecification(10, 10, 10, 5),
            Currency.EGP,
            DateTime.UtcNow.AddDays(1),
            "Delivery Address",
            "Pickup Address",
            enShipmentType.Standard,
            TransportationMode.Land,
            null,
            125m,
            DateTime.UtcNow.AddDays(4));
        typeof(Shipment).GetProperty(nameof(Shipment.Sender))!.SetValue(shipment, sender);
        typeof(Shipment).GetProperty(nameof(Shipment.RowVersion))!.SetValue(shipment, new byte[] { 1 });
        fixture.Context.Shipments.Add(shipment);
        await fixture.Context.SaveChangesAsync();
        return shipment;
    }

    internal static async Task PrepareTripAsync(
        SqliteAppDbContextFixture fixture, Trip trip)
    {
        await EnsureSqliteRowVersionDefaultAsync(fixture, "Trips");
    }

    private static async Task EnsureSqliteRowVersionDefaultAsync(
        SqliteAppDbContextFixture fixture, string tableName)
    {
        var connection = fixture.Context.Database.GetDbConnection();
        await using var read = connection.CreateCommand();
        read.CommandText = "SELECT sql FROM sqlite_master WHERE type = 'table' AND name = $name";
        var parameter = read.CreateParameter();
        parameter.ParameterName = "$name";
        parameter.Value = tableName;
        read.Parameters.Add(parameter);
        var createSql = (string?)await read.ExecuteScalarAsync();
        if (createSql is null || createSql.Contains("RowVersion\" BLOB NOT NULL DEFAULT", StringComparison.Ordinal))
            return;

        var normalizedSql = Regex.Replace(
            createSql,
            "\\\"RowVersion\\\"\\s+BLOB\\s+NOT NULL",
            "\"RowVersion\" BLOB NOT NULL DEFAULT X'01'",
            RegexOptions.CultureInvariant);
        if (normalizedSql == createSql)
            throw new InvalidOperationException($"Could not normalize {tableName}.RowVersion for SQLite tests.");

        var oldTableName = $"__{tableName}_before_rowversion_default";
        await using var rebuild = connection.CreateCommand();
        rebuild.CommandText = $"""
            PRAGMA foreign_keys = OFF;
            PRAGMA legacy_alter_table = ON;
            ALTER TABLE "{tableName}" RENAME TO "{oldTableName}";
            {normalizedSql};
            INSERT INTO "{tableName}" SELECT * FROM "{oldTableName}";
            DROP TABLE "{oldTableName}";
            PRAGMA legacy_alter_table = OFF;
            PRAGMA foreign_keys = ON;
            """;
        await rebuild.ExecuteNonQueryAsync();
    }
}
