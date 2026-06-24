using Microsoft.Extensions.DependencyInjection;
namespace TransitNova.InfraStructure.ServiceRegistration.PermissionsRegistration
{
    internal static  class PoliciesRegistrationExtensions
    {
        public static IServiceCollection AddPermissionPolicies(this IServiceCollection services , params string[] permissions)
        {
            services.AddAuthorization(options =>
            {
                foreach (var permission in permissions)
                {
                    options.AddPolicy(
                        permission,
                        policy => policy
                        .RequireClaim("Permission", permission)
                        .RequireAuthenticatedUser());

                }
            });

            return services;
        }
    }
    }

