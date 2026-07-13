using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace TransitNova.InfraStructure.ServiceRegistration.InfraStructureRegistration
{
    internal static class CorsRegistrationExtensions
    {
        public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var mvcHost = configuration["MVC:Host"];

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.WithOrigins(mvcHost!)
                          .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                          .WithHeaders("Authorization", "Content-Type", "Accept", "X-Requested-With", "X-Idempotency-Key", "X-SignalR-User-Agent")
                          .AllowCredentials();
                });
            });

            return services;
        }
    }

}
