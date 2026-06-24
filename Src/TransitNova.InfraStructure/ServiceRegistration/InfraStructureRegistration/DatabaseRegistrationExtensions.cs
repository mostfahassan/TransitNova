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

                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).AddInterceptors(interceptor!);
            });
            services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),lifetime:ServiceLifetime.Scoped);
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Dataabase Health Checking",HealthStatus.Unhealthy);
            return services;
        }
    }

}
