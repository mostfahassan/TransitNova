using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Models.PaymentHistoryEntity;
namespace TransitNovaPayment.Infrastructure.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : DbContext(options)
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentHistory> PaymentHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
            foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
                        .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                       {
                           property.SetPrecision(18);
                           property.SetScale(2);
                       }
        }

        private string GetConnectionString()
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