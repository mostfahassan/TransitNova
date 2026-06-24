
using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.InfraStructure.Repository;
using TransitNova.InfraStructure.Repository.Generic;

namespace TransitNova.InfraStructure.ServiceRegistration.GenericsRegistration
{
    public static class GenericRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddGenericRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>))
                .AddScoped<IUnitOfWork ,UnitOfWork> ();

            return services;
        }
    }
}
