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
using TransitNova.InfraStructure.Context;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

public sealed class TransitNovaWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly SemaphoreSlim _databaseInitializationLock = new(1, 1);
    private bool _databaseInitialized;

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

            services.AddSingleton(_connection);
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
            services.AddDbContextFactory<AppDbContext>(
                options => options.UseSqlite(_connection),
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
            _databaseInitialized = true;
        }
        finally
        {
            _databaseInitializationLock.Release();
        }
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
