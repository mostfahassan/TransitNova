using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.InfraStructure.Repository.CarrierRepo;
namespace TransitNova.InfraStructure.ServiceRegistration.CarrierRegistration
{
    public static class CarrierRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddCarrierRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<ICarrierQueryRepository, CarrierQueryRepository>()
                    .AddScoped<ICarrierShipmentQueryRepository, CarrierShipmentQueryRepository>()
                    .AddScoped<ICarrierAnalyticsQueryRepository, CarrierAnalyticsQueryRepository>()
                    .AddScoped<ICarrierRulesRepository, CarrierRulesValidation>()
                    .AddScoped<ICarrierDashboardRepository, CarrierDashboardRepository>()
                    .AddScoped<ICarrierCommandRepository, CarrierCommandRepository>();

            return services;
        }
    }

}
