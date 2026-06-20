using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.InfraStructure.Repository.TripRepository;

namespace TransitNova.InfraStructure.ServiceRegistration.TripRegistration
{
    public static class TripRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddTripRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITripQueryRepository, TripQueryRepository>()
                    .AddScoped<ITripCommandRepository, TripCommandRepository>();

            return services;
        }
    }
}
