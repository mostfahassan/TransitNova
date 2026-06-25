using Microsoft.EntityFrameworkCore;
using TransitNovaPayment.Infrastructure.Context;

namespace TransitNovaPayment.API
{
    public static class DatabaseSyncronyzation
    {
    public static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
    {
    if (app.Environment.IsEnvironment("Testing"))
        return;

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    const int maxAttempts = 10;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();

            if (pendingMigrations.Count == 0)
            {
                logger.LogInformation("Payment database is up to date. No pending migrations were found.");
                return;
            }

            logger.LogInformation(
                "Applying {MigrationCount} pending payment database migration(s): {Migrations}",
                pendingMigrations.Count,
                string.Join(", ", pendingMigrations));

            await context.Database.MigrateAsync();

            logger.LogInformation("Payment database migrations applied successfully.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(
                ex,
                "Payment database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in 5 seconds.",
                attempt,
                maxAttempts);

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payment database migration failed after {MaxAttempts} attempts.", maxAttempts);
            throw;
        }
    }
}
    }
}