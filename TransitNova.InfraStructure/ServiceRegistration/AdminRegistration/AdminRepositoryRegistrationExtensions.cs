using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.Admin;
using TransitNova.InfraStructure.Repository.Admin;
namespace TransitNova.InfraStructure.ServiceRegistration.AdminRegistration
{
    public static class AdminRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddAdminRepositories(
         this IServiceCollection services)
        {
            services.AddScoped<IAdminRulesRepository, AdminRulesRepository>()
                    .AddScoped<IAdminQueryRepository, AdminRulesRepository>()
                    .AddScoped<IAdminActivityQueryRepository, AdminActivityQueryRepository>()
                    .AddScoped<IAdminOperationalHealth, AdminOperationalHealth>()
                    .AddScoped<IAdminStatisticsQueryRepository, AdminStatisticsQueryRepository>();

            return services;
        }
    }
}
