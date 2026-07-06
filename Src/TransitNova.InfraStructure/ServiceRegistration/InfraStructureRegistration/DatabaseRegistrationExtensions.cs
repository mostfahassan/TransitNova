using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TransitNova.InfraStructure.Common.Interceptors;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Health;
namespace TransitNova.InfraStructure.ServiceRegistration.InfraStructureRegistration
{
    public static class DatabaseRegistrationExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ConvertDomainEventsToOutboxMessages>();
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetService<ConvertDomainEventsToOutboxMessages>();
                var connectionString = GetConnectionString(configuration);

                options.UseSqlServer(connectionString).AddInterceptors(interceptor!);
            });
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                var connectionString = GetConnectionString(configuration);
                options.UseSqlServer(connectionString);
            }, lifetime: ServiceLifetime.Scoped);
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database Health Check", HealthStatus.Unhealthy)
                .AddCheck<PaymentGatewayConfigurationHealthCheck>("Payment Gateway Configuration", HealthStatus.Unhealthy)
                .AddCheck<ObservabilityConfigurationHealthCheck>("Observability Configuration", HealthStatus.Unhealthy);
            return services;
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var connectionName = IsRunningInContainer() ? "DefaultConnection" : "HostContainerConnection";
            var connectionString = configuration.GetConnectionString(connectionName)
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

            return AddSqlPasswordIfMissing(connectionString);
        }

        private static string AddSqlPasswordIfMissing(string connectionString)
        {
            if (connectionString.Contains("Password=", StringComparison.OrdinalIgnoreCase)
                || connectionString.Contains("Pwd=", StringComparison.OrdinalIgnoreCase)
                || connectionString.Contains("Trusted_Connection=True", StringComparison.OrdinalIgnoreCase)
                || connectionString.Contains("Integrated Security=True", StringComparison.OrdinalIgnoreCase))
            {
                return connectionString;
            }

            var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? ReadEnvValue("SQL_PASSWORD");
            return string.IsNullOrWhiteSpace(password)
                ? connectionString
                : $"{connectionString.TrimEnd(';')};Password={password}";
        }

        private static bool IsRunningInContainer() =>
            string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);

        private static string? ReadEnvValue(string key)
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (currentDirectory is not null)
            {
                var envPath = Path.Combine(currentDirectory.FullName, ".env");
                if (File.Exists(envPath))
                {
                    foreach (var line in File.ReadLines(envPath))
                    {
                        var trimmed = line.Trim();
                        if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                            continue;

                        var separatorIndex = trimmed.IndexOf('=');
                        if (separatorIndex <= 0)
                            continue;

                        var name = trimmed[..separatorIndex].Trim();
                        if (!string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
                            continue;

                        return trimmed[(separatorIndex + 1)..].Trim().Trim('"');
                    }
                }

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }
    }

}