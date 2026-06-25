using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.InfraStructure.Repository.SystemActivityLogs;
using TransitNova.InfraStructure.ServiceRegistration.AdminRegistration;
using TransitNova.InfraStructure.ServiceRegistration.BackgroundJobsRegistration;
using TransitNova.InfraStructure.ServiceRegistration.BundleSubscriptionRegistration;
using TransitNova.InfraStructure.ServiceRegistration.CacheRegistration;
using TransitNova.InfraStructure.ServiceRegistration.CarrierRegistration;
using TransitNova.InfraStructure.ServiceRegistration.GenericsRegistration;
using TransitNova.InfraStructure.ServiceRegistration.InfraStructureRegistration;
using TransitNova.InfraStructure.ServiceRegistration.LocationRegistration;
using TransitNova.InfraStructure.ServiceRegistration.NotificationRegistration;
using TransitNova.InfraStructure.ServiceRegistration.OperationManagerRegistration;
using TransitNova.InfraStructure.ServiceRegistration.PermissionsRegistration;
using TransitNova.InfraStructure.ServiceRegistration.ShipmentRegistration;
using TransitNova.InfraStructure.ServiceRegistration.StrategyRegistration;
using TransitNova.InfraStructure.ServiceRegistration.TokenRegistration;
using TransitNova.InfraStructure.ServiceRegistration.TripRegistration;
using TransitNova.InfraStructure.ServiceRegistration.UserRepositoryRegistration;
using TransitNova.InfraStructure.ServiceRegistration.VehicleRepositoryRegistration;
using TransitNova.InfraStructure.ServiceRegistration.WarehouseRegistration;
namespace TransitNova.InfraStructure.ServiceRegistration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraStructureService(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services
                .AddDatabase(configuration)
                .AddCorsPolicies(configuration)
                .AddInfrastructureIdentity()
                .AddRoleService()
                .AddJwtAuthentication(configuration)
                .AddAuthorization()
                .AddBackgroundJobServices()
                .AddCacheServices()
                .AddPermissionPolicies(
                [
                    ..UserPermissions.All,
                    ..CarrierPermissions.All,
                    ..OperationManagerPermissions.All,
                    ..AdminPermissions.All
                ]);

            services
                .AddGenericRepositories()
                .AddShipmentRepositories()
                .AddCarrierRepositories()
                .AddVehicleRepositories()
                .AddTripRepositories()
                .AddUserRepositories()
                .AddAdminRepositories()
                .AddCarrierRatingRepositories()
                .AddOperationManagerRepositories()
                .AddWarehouseRepositories()
                .AddLocationRepositories()
                .AddNotificationServices()
                .AddBundleSubscriptionRepositories();

            services.AddScoped<ISystemLogCommands, SystemLogCommands>();
            
            services
                .AddRegistrationStrategies()
                .AddTokenServices();

            return services;
        }

       
    }
}
