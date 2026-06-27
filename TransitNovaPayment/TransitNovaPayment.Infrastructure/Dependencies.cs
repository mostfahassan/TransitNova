using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Infrastructure.Context;
using TransitNovaPayment.Infrastructure.RepositoryImplementation;
using TransitNovaPayment.Infrastructure.RepositoryImplementation.PaymentRepo;
using TransitNovaPayment.Infrastructure.Service.Caching;
using TransitNovaPayment.InfraStructure.Health;
namespace TransitNovaPayment.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(
            this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            services.AddScoped<IPaymentCommandRepository, PaymentCommandRepository>();
            services.AddScoped<IPaymentQueryRepository, PaymentQueryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database Health Check", HealthStatus.Unhealthy)
                .AddCheck<PaymentGatewayConfigurationHealthCheck>("Payment Gateway Configuration", HealthStatus.Unhealthy)
                .AddCheck<ObservabilityConfigurationHealthCheck>("Observability Configuration", HealthStatus.Unhealthy);
            return services;
        }
    }
}