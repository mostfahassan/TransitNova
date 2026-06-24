using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository;
using TransitNova.InfraStructure.Repository.WarehouseRepo;
namespace TransitNova.InfraStructure.ServiceRegistration.WarehouseRegistration
{
    public static class WarehouseRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddWarehouseRepositories(this IServiceCollection services)
        {
            services.AddScoped<IWarehouseQueriesRepository, WarehouseRepository>()
                     .AddScoped<IWarehouseCommandsRepository, WarehouseCommandRepository>()
                     .AddScoped<IWarehouseRulesRepository, WarehouseRulesRepository>();

            return services;
        }
    }
}
