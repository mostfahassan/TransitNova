using Microsoft.EntityFrameworkCore;
using TransitNova.Api;
using TransitNova.InfraStructure.Context;

var builder = WebApplication.CreateBuilder(args);
// Create services to the container.
#region Services
builder.Services.AddDependencies(builder.Configuration);
builder.Host.AddSerilog();
#endregion

var app = builder.Build();

await ApplyDatabaseMigrationsAsync(app);

app.UseDependencies();
app.Run();






static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
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
                logger.LogInformation("Database is up to date. No pending migrations were found.");
                return;
            }

            logger.LogInformation(
                "Applying {MigrationCount} pending database migration(s): {Migrations}",
                pendingMigrations.Count,
                string.Join(", ", pendingMigrations));

            await context.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(
                ex,
                "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in 5 seconds.",
                attempt,
                maxAttempts);

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed after {MaxAttempts} attempts.", maxAttempts);
            throw;
        }
    }
}


