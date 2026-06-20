using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.ServiceRegistration.InfraStructureRegistration
{
    public static class DatabaseRegistrationExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContextFactory<AppDbContext>(
                    options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                    ServiceLifetime.Scoped);

            return services;
        }
    }
    
}
