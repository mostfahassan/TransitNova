using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;
using TransitNova.InfraStructure;
using TransitNova.InfraStructure.Context;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

public sealed class TransitNovaWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString = $"Data Source=TransitNovaTests-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _connection;
    private readonly SemaphoreSlim _databaseInitializationLock = new(1, 1);
    private bool _databaseInitialized;

    public TransitNovaWebApplicationFactory()
    {
        _connection = new SqliteConnection(_connectionString);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT:Key"] = "TransitNova-Integration-Tests-Signing-Key-2026-At-Least-64-Characters",
                ["JWT:Issuer"] = "TransitNova.IntegrationTests",
                ["JWT:Audience"] = "TransitNova.IntegrationTests",
                ["MVC:Host"] = "https://localhost",
                ["PaymentSettings:PublicKey"] = "integration-payment-public-key",
                ["PaymentSettings:BaseUrl"] = "https://payments.local"
            });
        });

        builder.ConfigureServices(services =>
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextFactory<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connectionString));
            services.AddDbContextFactory<AppDbContext>(
                options => options.UseSqlite(_connectionString),
                ServiceLifetime.Scoped);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultForbidScheme = TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName,
                    _ => { })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    IdentityConstants.ApplicationScheme,
                    _ => { });

            services.AddSingleton<IAuthorizationHandler, AllowTestAuthorizationHandler>();
        });
    }

    internal HttpClient CreateAuthenticatedClient(string? userName = null)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.AuthenticationHeader, "true");
        client.DefaultRequestHeaders.Add(
            TestAuthenticationHandler.UserHeader,
            userName ?? $"integration-user-{Guid.NewGuid():N}");
        return client;
    }

    internal HttpClient CreateAnonymousClient() =>
        CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

    internal async Task InitializeDatabaseAsync()
    {
        await _databaseInitializationLock.WaitAsync();
        try
        {
            if (_databaseInitialized)
                return;

            _ = Server;
            await using var scope = Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
            await SeedAuthenticatedWarehouseManagerAsync(context);
            _databaseInitialized = true;
        }
        finally
        {
            _databaseInitializationLock.Release();
        }
    }


    private static async Task SeedAuthenticatedWarehouseManagerAsync(AppDbContext context)
    {
        if (await context.WarehouseManagersProfiles.AnyAsync(
                manager => manager.AppUserId == TestAuthenticationHandler.UserId))
            return;

        var country = Country.Create("Integration Country");
        context.Countries.Add(country);
        await context.SaveChangesAsync();

        var government = Government.Create("Integration Government", country.Id);
        context.Governments.Add(government);
        await context.SaveChangesAsync();

        var city = City.Create("Integration City", government.Id);
        context.Cities.Add(city);
        await context.SaveChangesAsync();

        if (!await context.AppUsers.AnyAsync(user => user.Id == TestAuthenticationHandler.UserId))
        {
            context.AppUsers.Add(new AppUser
            {
                Id = TestAuthenticationHandler.UserId,
                UserName = "integration-warehouse-manager",
                NormalizedUserName = "INTEGRATION-WAREHOUSE-MANAGER",
                Email = "integration-warehouse-manager@transitnova.test",
                NormalizedEmail = "INTEGRATION-WAREHOUSE-MANAGER@TRANSITNOVA.TEST",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                UserType = UserType.WarehouseManager,
                FullName = "Integration Warehouse Manager"
            });
            await context.SaveChangesAsync();
        }

        var manager = WarehouseManagerProfile.Create(
            TestAuthenticationHandler.UserId,
            "Integration",
            "Warehouse Manager",
            "integration-warehouse-manager@transitnova.test",
            "01000000000",
            "Integration Address",
            city.Id);

        context.WarehouseManagersProfiles.Add(manager);
        await context.SaveChangesAsync();

        var warehouse = Warehouse.Create(
            "Integration Warehouse",
            WarehouseType.MainWarehouse,
            1000,
            0,
            24,
            "Integration Warehouse Address",
            TestAuthenticationHandler.UserId,
            manager.Id);

        await context.Database.ExecuteSqlInterpolatedAsync($$"""
            INSERT INTO Warehouses
                (Id, Name, Type, HasManager, Address, RowVersion, Capacity, CurrentUsage, ManagerId, OperatingHours, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, CurrentState)
            VALUES
                ({{warehouse.Id}}, {{warehouse.Name}}, {{warehouse.Type}}, {{warehouse.HasManager}}, {{warehouse.Address}}, X'01', {{warehouse.Capacity}}, {{warehouse.CurrentUsage}}, {{warehouse.ManagerId}}, {{warehouse.OperatingHours}}, {{DateTime.UtcNow}}, {{warehouse.CreatedBy}}, NULL, NULL, {{warehouse.CurrentState}});
            """);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _databaseInitializationLock.Dispose();
            _connection.Dispose();
        }
    }
}
