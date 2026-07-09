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
using Microsoft.Extensions.Hosting;
using System.Data;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;
using TransitNova.InfraStructure;
using TransitNova.InfraStructure.Context;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

public sealed class TransitNovaWebApplicationFactory : WebApplicationFactory<Program>
{
    internal const string KnownUserEmail = "contract-user@transitnova.test";
    internal const string KnownUserPassword = "Pass@123";
    private static readonly Guid AvailableWarehouseManagerUserId = Guid.Parse("77777777-7777-7777-7777-777777777777");

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

            services.RemoveAll<IHostedService>();

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

    internal HttpClient CreateAuthenticatedClient(
        string? userName = null,
        IEnumerable<string>? roles = null,
        IEnumerable<string>? permissions = null,
        bool bypassAuthorization = true)
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

        if (roles is not null)
            client.DefaultRequestHeaders.Add(
                TestAuthenticationHandler.RolesHeader,
                string.Join(',', roles));

        if (permissions is not null)
            client.DefaultRequestHeaders.Add(
                TestAuthenticationHandler.PermissionsHeader,
                string.Join(',', permissions));

        if (!bypassAuthorization)
            client.DefaultRequestHeaders.Add(TestAuthenticationHandler.BypassAuthorizationHeader, "false");

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
            await SeedIdentityRolesAsync(context);
            var cityId = await SeedAuthenticatedWarehouseManagerAsync(context);
            await SeedAuthenticatedAdminAsync(context, cityId);
            await SeedKnownLoginUserAsync(context);
            _databaseInitialized = true;
        }
        finally
        {
            _databaseInitializationLock.Release();
        }
    }

    internal async Task<int> GetAnyCityIdAsync()
    {
        await InitializeDatabaseAsync();
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.Cities
            .OrderBy(city => city.Id)
            .Select(city => city.Id)
            .FirstAsync();
    }

    internal async Task<Guid> GetWarehouseManagerProfileIdAsync()
    {
        await InitializeDatabaseAsync();
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.WarehouseManagersProfiles
            .Where(manager => !context.Warehouses.Any(warehouse => warehouse.ManagerId == manager.Id))
            .OrderBy(manager => manager.Id)
            .Select(manager => manager.Id)
            .FirstAsync();
    }

    private static async Task SeedIdentityRolesAsync(AppDbContext context)
    {
        var roles = new[]
        {
            Role.Admin,
            Role.User,
            Role.OperationManager,
            Role.Carrier,
            Role.WarehouseManager
        };

        foreach (var roleName in roles)
        {
            if (await context.Roles.AnyAsync(role => role.Name == roleName))
                continue;

            context.Roles.Add(new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant()
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedKnownLoginUserAsync(AppDbContext context)
    {
        if (await context.AppUsers.AnyAsync(user => user.Email == KnownUserEmail))
            return;

        var user = new AppUser
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserName = "contract-user",
            NormalizedUserName = "CONTRACT-USER",
            Email = KnownUserEmail,
            NormalizedEmail = KnownUserEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            UserType = UserType.User,
            FullName = "Contract Test User",
            PhoneNumber = "+201001234567"
        };

        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, KnownUserPassword);

        context.AppUsers.Add(user);
        await context.SaveChangesAsync();

        var userRoleId = await context.Roles
            .Where(role => role.Name == Role.User)
            .Select(role => role.Id)
            .SingleAsync();

        context.UserRoles.Add(new IdentityUserRole<Guid>
        {
            UserId = user.Id,
            RoleId = userRoleId
        });

        await context.SaveChangesAsync();
    }

    private static async Task<int> SeedAuthenticatedWarehouseManagerAsync(AppDbContext context)
    {
        var existingCityId = await context.Cities
            .OrderBy(city => city.Id)
            .Select(city => (int?)city.Id)
            .FirstOrDefaultAsync();

        var cityId = existingCityId;
        if (cityId is null)
        {
            var country = Country.Create("Integration Country");
            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var government = Government.Create("Integration Government", country.Id);
            context.Governments.Add(government);
            await context.SaveChangesAsync();

            var city = City.Create("Integration City", government.Id);
            context.Cities.Add(city);
            await context.SaveChangesAsync();
            cityId = city.Id;
        }

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
                FullName = "Integration Warehouse Manager",
                PhoneNumber = "+201000000000"
            });
            await context.SaveChangesAsync();
        }

        if (!await context.WarehouseManagersProfiles.AnyAsync(manager => manager.AppUserId == TestAuthenticationHandler.UserId))
        {
            var manager = WarehouseManagerProfile.Create(
                TestAuthenticationHandler.UserId,
                "Integration",
                "Warehouse Manager",
                "integration-warehouse-manager@transitnova.test",
                "01000000000",
                "Integration Address",
                cityId.Value);

            context.WarehouseManagersProfiles.Add(manager);
            await context.SaveChangesAsync();
        }

        if (!await context.OperationManagerProfiles.AnyAsync(operationManager => operationManager.AppUserId == TestAuthenticationHandler.UserId))
        {
            var operationManager = OperationManagerProfile.Create(
                TestAuthenticationHandler.UserId,
                "Integration",
                "Operation Manager",
                "integration-operation-manager@transitnova.test",
                "01000000001",
                "Integration Address",
                cityId.Value);

            context.OperationManagerProfiles.Add(operationManager);
            await context.SaveChangesAsync();
        }

        if (!await context.AppUsers.AnyAsync(user => user.Id == AvailableWarehouseManagerUserId))
        {
            context.AppUsers.Add(new AppUser
            {
                Id = AvailableWarehouseManagerUserId,
                UserName = "available-warehouse-manager",
                NormalizedUserName = "AVAILABLE-WAREHOUSE-MANAGER",
                Email = "available-warehouse-manager@transitnova.test",
                NormalizedEmail = "AVAILABLE-WAREHOUSE-MANAGER@TRANSITNOVA.TEST",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                UserType = UserType.WarehouseManager,
                FullName = "Available Warehouse Manager",
                PhoneNumber = "+201000000001"
            });
            await context.SaveChangesAsync();
        }

        if (!await context.WarehouseManagersProfiles.AnyAsync(manager => manager.AppUserId == AvailableWarehouseManagerUserId))
        {
            var availableManager = WarehouseManagerProfile.Create(
                AvailableWarehouseManagerUserId,
                "Available",
                "Manager",
                "available-warehouse-manager@transitnova.test",
                "01000000002",
                "Integration Address",
                cityId.Value);

            context.WarehouseManagersProfiles.Add(availableManager);
            await context.SaveChangesAsync();
        }

        var assignedManagerId = await context.WarehouseManagersProfiles
            .Where(manager => manager.AppUserId == TestAuthenticationHandler.UserId)
            .Select(manager => manager.Id)
            .FirstAsync();

        if (!await context.Warehouses.AnyAsync(warehouse => warehouse.ManagerId == assignedManagerId))
        {
            var warehouse = Warehouse.Create(
                "Integration Warehouse",
                WarehouseType.MainWarehouse,
                1000,
                0,
                24,
                "Integration Warehouse Address",
                TestAuthenticationHandler.UserId,
                assignedManagerId);

            await context.Database.ExecuteSqlInterpolatedAsync($$"""
                INSERT INTO Warehouses
                    (Id, Name, Type, HasManager, Address, RowVersion, Capacity, CurrentUsage, ManagerId, OperatingHours, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, CurrentState)
                VALUES
                    ({{warehouse.Id}}, {{warehouse.Name}}, {{warehouse.Type}}, {{warehouse.HasManager}}, {{warehouse.Address}}, X'01', {{warehouse.Capacity}}, {{warehouse.CurrentUsage}}, {{warehouse.ManagerId}}, {{warehouse.OperatingHours}}, {{DateTime.UtcNow}}, {{warehouse.CreatedBy}}, NULL, NULL, {{warehouse.CurrentState}});
                """);
        }

        return cityId.Value;
    }

    private static async Task SeedAuthenticatedAdminAsync(AppDbContext context, int cityId)
    {
        if (await context.Admins.AnyAsync(admin => admin.AppUserId == TestAuthenticationHandler.UserId))
            return;

        var admin = AdminProfile.Create(
            TestAuthenticationHandler.UserId,
            "Integration",
            "Admin",
            "integration-admin@transitnova.test",
            "+201055555555",
            "Integration Address",
            cityId);

        context.Admins.Add(admin);
        await context.SaveChangesAsync();
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




