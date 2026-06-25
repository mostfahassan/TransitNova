using Microsoft.Extensions.Diagnostics.HealthChecks;
using TransitNovaPayment.Infrastructure.Context;
namespace TransitNovaPayment.InfraStructure.Health
{
    internal class DatabaseHealthCheck(AppDbContext db ) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await db.Database.CanConnectAsync(cancellationToken)
                 ? HealthCheckResult.Healthy("Database Connection Is Health")
                 : HealthCheckResult.Unhealthy("Database Connection Is UnHealthy");
        }
    }
}
