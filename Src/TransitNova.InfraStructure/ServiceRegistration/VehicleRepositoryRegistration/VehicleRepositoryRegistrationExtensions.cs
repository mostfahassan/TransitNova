using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.InfraStructure.Repository.VehicleRepo;
using TransitNova.InfraStructure.Repository.VehicleRepo.VehicleRules;

namespace TransitNova.InfraStructure.ServiceRegistration.VehicleRepositoryRegistration
{
    public static class VehicleRepositoryRegistrationExtensions
    {

        public static IServiceCollection AddVehicleRepositories(this IServiceCollection services)
        {
            services.AddScoped<IVehicleQueryRepository, VehicleQueryRepository>();
            services.AddScoped<IVehicleRulesRepository, VehicleRulesRepository>();

            return services;

        }
    }
}
