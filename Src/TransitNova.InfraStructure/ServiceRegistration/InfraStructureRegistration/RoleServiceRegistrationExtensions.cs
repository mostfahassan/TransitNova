
using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
using TransitNova.InfraStructure.Common.RolesService;
namespace TransitNova.InfraStructure.ServiceRegistration.InfraStructureRegistration
{
    public static class RoleServiceRegistrationExtensions
    {
        public static IServiceCollection AddRoleService(this IServiceCollection services)
        {
            services.AddScoped<IRolesCommandsService, RolesCommandsService>()
                .AddScoped<IRolesQueryService, RolesQueryService>()
                .AddScoped<IRoleAssignmentService, RoleAssignmentService>();

            return services;
        }
    }
    
}
