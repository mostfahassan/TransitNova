using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;
namespace TransitNova.InfraStructure.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private const string ContainerConnectionString = "Server=sqlserver,1433;Database=TransitNovaDb;User Id=sa;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True";
        private const string HostContainerConnectionString = "Server=localhost,1433;Database=TransitNovaDb;User Id=sa;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True";

        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            return new AppDbContext(optionsBuilder.Options);
        }

        private static string GetConnectionString()
        {
            var environmentConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            if (!string.IsNullOrWhiteSpace(environmentConnectionString))
                return environmentConnectionString;

            var configuredConnectionString = ReadConnectionStringFromAppsettings() ?? ResolveFallbackConnectionString();
            return AddSqlPasswordIfMissing(configuredConnectionString);
        }

        private static string? ReadConnectionStringFromAppsettings()
        {
            var appsettingsPath = FindAppsettingsPath();
            if (appsettingsPath is null)
                return null;

            var connectionName = IsRunningInContainer() ? "DefaultConnection" : "HostContainerConnection";

            using var document = JsonDocument.Parse(File.ReadAllText(appsettingsPath));
            if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
                && connectionStrings.TryGetProperty(connectionName, out var preferredConnection)
                && !string.IsNullOrWhiteSpace(preferredConnection.GetString()))
            {
                return preferredConnection.GetString();
            }

            if (connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
                return defaultConnection.GetString();

            return null;
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

        private static string ResolveFallbackConnectionString() =>
            IsRunningInContainer() ? ContainerConnectionString : HostContainerConnectionString;

        private static bool IsRunningInContainer() =>
            string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);

        private static string? FindAppsettingsPath()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (currentDirectory is not null)
            {
                var apiAppsettings = Path.Combine(currentDirectory.FullName, "Src", "TransitNova.Api", "appsettings.json");
                if (File.Exists(apiAppsettings))
                    return apiAppsettings;

                var localAppsettings = Path.Combine(currentDirectory.FullName, "appsettings.json");
                if (File.Exists(localAppsettings))
                    return localAppsettings;

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }

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