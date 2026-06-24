using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.InfraStructure.Repository.VehicleRepo;

namespace TransitNova.InfraStructure.ServiceRegistration.VehicleRegistration
{
    public static class VehicleRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddVehicleRepositories(this IServiceCollection services)
        {
            services.AddScoped<IVehicleQueryRepository, VehicleQueryRepository>()
                    .AddScoped<IVehicleRulesRepository, VehicleRulesRepository>();

            return services;
        }
    }
}
