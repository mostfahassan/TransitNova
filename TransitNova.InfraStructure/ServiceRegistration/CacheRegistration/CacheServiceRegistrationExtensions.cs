using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.InfraStructure.Common.CacheService;
namespace TransitNova.InfraStructure.ServiceRegistration.CacheRegistration
{
    public static class CacheServiceRegistrationExtensions
    {
        public static IServiceCollection AddCacheServices(
            this IServiceCollection services)
        {
            services.AddScoped<ICacheService, MemoryCacheService>();
                
            return services;
        }
    }
}
