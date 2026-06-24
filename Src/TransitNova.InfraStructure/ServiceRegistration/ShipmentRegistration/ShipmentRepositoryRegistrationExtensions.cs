using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.InfraStructure.Repository.ShipmentRepo;

namespace TransitNova.InfraStructure.ServiceRegistration.ShipmentRegistration
{
    public static class ShipmentRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddShipmentRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IShipmentCommandRepository, ShipmentCommandRepository>()
                    .AddScoped<IShipmentQueryRepository, ShipmentQueryRepository>()
                    .AddScoped<IShipmentRulesRepository, ShipmentRulesRepository>();

            return services;
        }
    }
}
