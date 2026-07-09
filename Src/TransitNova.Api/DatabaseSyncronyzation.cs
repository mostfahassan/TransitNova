using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransitNova.Domain.Enums.Users;
using TransitNova.InfraStructure;
using TransitNova.InfraStructure.Context;
using TransitNova.InfraStructure.Context.Seeder;

namespace TransitNova.Api
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

                    if (pendingMigrations.Count > 0)
                    {
                        logger.LogInformation(
                            "Applying {MigrationCount} pending database migration(s): {Migrations}",
                            pendingMigrations.Count,
                            string.Join(", ", pendingMigrations));

                        await context.Database.MigrateAsync();

                        logger.LogInformation("Database migrations applied successfully.");
                    }
                    else
                    {
                        logger.LogInformation("Database is up to date. No pending migrations were found.");
                    }

                    await SeedIdentityRolesAsync(services, logger);

                    if (app.Configuration.GetValue("SeedDemoData", app.Environment.IsDevelopment()))
                    {
                        await DatabaseSeeder.SeedDemoDataAsync(services, logger);
                    }

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

        private static async Task SeedIdentityRolesAsync(IServiceProvider services, ILogger logger)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var roleNames = Enum.GetNames<UserType>()
                .Where(roleName => roleName != nameof(UserType.Receiver));

            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (result.Succeeded)
                {
                    logger.LogInformation("Seeded identity role {RoleName}.", roleName);
                    continue;
                }

                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed identity role '{roleName}': {errors}");
            }
        }
    }
}
