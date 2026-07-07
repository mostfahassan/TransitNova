using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Bundles.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Bundles.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Carriers.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Carriers.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Cities.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Cities.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Dashboard.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Governments.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Governments.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.OperationManagers.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Roles.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Roles.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Subscriptions.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Trips.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Users.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Vehicles.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Vehicles.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.WarehouseManagers.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Warehouses.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Warehouses.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Trips.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.WarehouseManagers.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.WarehouseManagers.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.AdminRegistration;

public static class AdminApiClientRegistrationExtensions
{
    public static IServiceCollection AddAdminApiClients(this IServiceCollection services)
    {
        services.AddScoped<AdminBundlesCommand>();
        services.AddScoped<IAdminBundlesCommand, AdminBundlesCommand>();
        services.AddScoped<ICreateBundleCommandService, AdminBundlesCommand>();
        services.AddScoped<IDeleteBundleCommandService, AdminBundlesCommand>();
        services.AddScoped<IUpdateBundleCommandService, AdminBundlesCommand>();

        services.AddScoped<AdminBundlesQuery>();
        services.AddScoped<IAdminBundlesQuery, AdminBundlesQuery>();
        services.AddScoped<IGetBundleByIdQueryService, AdminBundlesQuery>();
        services.AddScoped<IGetBundlesQueryService, AdminBundlesQuery>();

        services.AddScoped<AdminCarriersCommand>();
        services.AddScoped<IAdminCarriersCommand, AdminCarriersCommand>();
        services.AddScoped<IDeleteCarrierCommandService, AdminCarriersCommand>();

        services.AddScoped<AdminCarriersQuery>();
        services.AddScoped<IAdminCarriersQuery, AdminCarriersQuery>();
        services.AddScoped<IGetAdminCarrierByIdQueryService, AdminCarriersQuery>();
        services.AddScoped<IGetAdminCarrierShipmentByIdQueryService, AdminCarriersQuery>();
        services.AddScoped<IGetAdminCarrierShipmentsQueryService, AdminCarriersQuery>();
        services.AddScoped<IGetAdminCarriersQueryService, AdminCarriersQuery>();

        services.AddScoped<AdminCitiesCommand>();
        services.AddScoped<IAdminCityCommand, AdminCitiesCommand>();
        services.AddScoped<ICreateCityCommandService, AdminCitiesCommand>();
        services.AddScoped<IDeleteCityCommandService, AdminCitiesCommand>();
        services.AddScoped<IUpdateCityCommandService, AdminCitiesCommand>();

        services.AddScoped<AdminCitiesQuery>();
        services.AddScoped<IAdminCityQuery, AdminCitiesQuery>();
        services.AddScoped<IFilterCitiesQueryService, AdminCitiesQuery>();
        services.AddScoped<IGetCityByIdQueryService, AdminCitiesQuery>();

        services.AddScoped<AdminDashboardQuery>();
        services.AddScoped<IAdminDashboardQuery, AdminDashboardQuery>();
        services.AddScoped<IGetAdminDashboardQueryService, AdminDashboardQuery>();

        services.AddScoped<AdminGovernmentsCommand>();
        services.AddScoped<IAdminGovernmentCommand, AdminGovernmentsCommand>();
        services.AddScoped<ICreateGovernmentCommandService, AdminGovernmentsCommand>();
        services.AddScoped<IDeleteGovernmentCommandService, AdminGovernmentsCommand>();
        services.AddScoped<IUpdateGovernmentCommandService, AdminGovernmentsCommand>();

        services.AddScoped<AdminGovernmentsQuery>();
        services.AddScoped<IAdminGovernmentQuery, AdminGovernmentsQuery>();
        services.AddScoped<IGetGovernmentByIdQueryService, AdminGovernmentsQuery>();
        services.AddScoped<IGetGovernmentsQueryService, AdminGovernmentsQuery>();

        services.AddScoped<AdminOperationManagersQuery>();
        services.AddScoped<IAdminOperationManagerQuery, AdminOperationManagersQuery>();
        services.AddScoped<IGetActiveOperationManagersQueryService, AdminOperationManagersQuery>();
        services.AddScoped<IGetOperationManagerByIdQueryService, AdminOperationManagersQuery>();
        services.AddScoped<IGetOperationManagerHandledCarriersQueryService, AdminOperationManagersQuery>();
        services.AddScoped<IGetOperationManagerHandledShipmentsQueryService, AdminOperationManagersQuery>();
        services.AddScoped<IGetOperationManagersQueryService, AdminOperationManagersQuery>();

        services.AddScoped<AdminRolesCommand>();
        services.AddScoped<IAdminRolesCommand, AdminRolesCommand>();
        services.AddScoped<ICreateRoleCommandService, AdminRolesCommand>();
        services.AddScoped<IDeleteRoleCommandService, AdminRolesCommand>();
        services.AddScoped<IUpdateRoleCommandService, AdminRolesCommand>();
        services.AddScoped<IUpdateRoleMembersCommandService, AdminRolesCommand>();

        services.AddScoped<AdminRolesQuery>();
        services.AddScoped<IAdminRolesQuery, AdminRolesQuery>();
        services.AddScoped<IGetRoleByIdQueryService, AdminRolesQuery>();
        services.AddScoped<IGetRoleMembersQueryService, AdminRolesQuery>();
        services.AddScoped<IGetRolesQueryService, AdminRolesQuery>();

        services.AddScoped<AdminSubscriptionsQuery>();
        services.AddScoped<IAdminSubscriptionQuery, AdminSubscriptionsQuery>();
        services.AddScoped<IGetBundleSubscribersQueryService, AdminSubscriptionsQuery>();
        services.AddScoped<IGetSubscriptionByIdQueryService, AdminSubscriptionsQuery>();

        services.AddScoped<AdminTripsQuery>();
        services.AddScoped<IAdminTripsQuery, AdminTripsQuery>();
        services.AddScoped<IGetAdminTripByIdQueryService, AdminTripsQuery>();
        services.AddScoped<IGetAdminTripsQueryService, AdminTripsQuery>();

        services.AddScoped<AdminUsersQuery>();
        services.AddScoped<IAdminUserQuery, AdminUsersQuery>();
        services.AddScoped<IFilterUsersQueryService, AdminUsersQuery>();
        services.AddScoped<IGetUserDetailsQueryService, AdminUsersQuery>();

        services.AddScoped<AdminVehiclesCommand>();
        services.AddScoped<IAdminVehiclesCommand, AdminVehiclesCommand>();
        services.AddScoped<ICreateVehicleCommandService, AdminVehiclesCommand>();
        services.AddScoped<IDeleteVehicleCommandService, AdminVehiclesCommand>();

        services.AddScoped<AdminVehiclesQuery>();
        services.AddScoped<IAdminVehiclesQuery, AdminVehiclesQuery>();
        services.AddScoped<IGetActiveVehiclesQueryService, AdminVehiclesQuery>();
        services.AddScoped<IGetVehicleByIdQueryService, AdminVehiclesQuery>();
        services.AddScoped<IGetVehicleByPlateNumberQueryService, AdminVehiclesQuery>();
        services.AddScoped<IGetVehiclesQueryService, AdminVehiclesQuery>();

        services.AddScoped<AdminWarehousesCommand>();
        services.AddScoped<IAdminWarehousesCommand, AdminWarehousesCommand>();
        services.AddScoped<ICreateWarehouseCommandService, AdminWarehousesCommand>();
        services.AddScoped<IDeleteWarehouseCommandService, AdminWarehousesCommand>();
        services.AddScoped<IUpdateWarehouseCommandService, AdminWarehousesCommand>();

        services.AddScoped<AdminWarehousesQuery>();
        services.AddScoped<IAdminWarehousesQuery, AdminWarehousesQuery>();
        services.AddScoped<IGetWarehouseByIdQueryService, AdminWarehousesQuery>();
        services.AddScoped<IGetWarehousesQueryService, AdminWarehousesQuery>();

        services.AddScoped<AdminWarehouseManagersQuery>();
        services.AddScoped<IAdminWarehouseManagersQuery, AdminWarehouseManagersQuery>();
        services.AddScoped<IGetWarehouseManagerByIdQueryService, AdminWarehouseManagersQuery>();
        services.AddScoped<IGetWarehouseManagersQueryService, AdminWarehouseManagersQuery>();

        return services;
    }
}