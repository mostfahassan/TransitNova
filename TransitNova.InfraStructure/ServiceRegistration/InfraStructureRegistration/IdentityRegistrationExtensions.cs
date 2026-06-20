using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.ServiceRegistration.InfraStructureRegistration
{
    internal static class IdentityRegistrationExtensions
    {
        public static IServiceCollection AddInfrastructureIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>()
                    .AddRoles<IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddSignInManager()
                    .AddDefaultTokenProviders();

            return services;
        }
    }
}
