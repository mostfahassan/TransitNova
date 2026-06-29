using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
using TransitNova.InfraStructure.Repository.WarehouseManagerRepo;
namespace TransitNova.InfraStructure.ServiceRegistration.WarehouseManagerRegistration
{
    public static class WarehouseManagerRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddWarehouseManagerRepositories(this IServiceCollection services)
        {
            services.AddScoped<IWarehouseManagerRuleseRepository, WarehouseManagerRulesRepository>()
                    .AddScoped<IWarehouseManagerQueryRepository, WarehouseManagerQueryRepository>()
                    .AddScoped<IWarehouseManagerDashboardRepository, WarehouseManagerDashboardRepository>()
                    .AddScoped<IWarehouseManagerCommandRepository, WarehouseManagerCommandRepository>();
            return services;
        }
    }
}
