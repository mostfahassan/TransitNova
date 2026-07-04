using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.InfraStructure.Repository.OperationManager;

namespace TransitNova.InfraStructure.ServiceRegistration.OperationManagerRegistration
{
    public static class OperationManagerRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddOperationManagerRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IOperationManagerQueryRepository, OperationManagerQueryRepository>()
                    .AddScoped<IOperationManagerRulesRepository, OperationManagerRulesRepository>()
                    .AddScoped<IOperationManagerDashboardRepository, OperationManagerDashboardRepository>()
                    .AddScoped<IOperationManagerCommandsRepository, OperationManagerCommandsRepository>();

            return services;
        }
    }
}
