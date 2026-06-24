using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRatingRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.InfraStructure.Repository.CarrierRatings;
using TransitNova.InfraStructure.Repository.CarrierRepo;
namespace TransitNova.InfraStructure.ServiceRegistration.CarrierRegistration
{
    public static class CarrierRatingRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddCarrierRatingRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<ICarrierRatingCommandsRepository, CarrierRatingCommandsRepository>();
            return services;
        }
    }

}
