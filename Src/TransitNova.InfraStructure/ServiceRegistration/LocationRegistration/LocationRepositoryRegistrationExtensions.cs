using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.InfraStructure.Repository.Location;

namespace TransitNova.InfraStructure.ServiceRegistration.LocationRegistration
{
    public static class LocationRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddLocationRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICityRepository, CityRepository>()
                    .AddScoped<ICountryRepository, CountryRepository>()
                    .AddScoped<IZoneRepository, ZoneRepository>();

            return services;
        }
    }
}
