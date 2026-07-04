using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Carriers.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Dashboard.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Dashboard.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Shipments.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Trips.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Shipments.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Trips.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Trips.Segregations.Query;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.WarehouseManagerRegistration;

public static class WarehouseManagerApiClientRegistrationExtensions
{
    public static IServiceCollection AddWarehouseManagerApiClients(this IServiceCollection services)
    {
        services.AddScoped<WarehouseManagerCarriersQuery>();
        services.AddScoped<IGetWarehouseManagerCarrierByIdQueryService, WarehouseManagerCarriersQuery>();
        services.AddScoped<IGetWarehouseManagerCarriersQueryService, WarehouseManagerCarriersQuery>();
        services.AddScoped<IWarehouseManagerCarriersQuery, WarehouseManagerCarriersQuery>();

        services.AddScoped<WarehouseManagerDashboardCommand>();
        services.AddScoped<IUpdateWarehouseManagerCommandService, WarehouseManagerDashboardCommand>();
        services.AddScoped<IWarehouseManagerDashboardCommand, WarehouseManagerDashboardCommand>();

        services.AddScoped<WarehouseManagerDashboardQuery>();
        services.AddScoped<IGetWarehouseManagerDashboardQueryService, WarehouseManagerDashboardQuery>();
        services.AddScoped<IWarehouseManagerDashboardQuery, WarehouseManagerDashboardQuery>();

        services.AddScoped<WarehouseManagerShipmentsQuery>();
        services.AddScoped<IGetWarehouseManagerShipmentByIdQueryService, WarehouseManagerShipmentsQuery>();
        services.AddScoped<IGetWarehouseManagerShipmentsQueryService, WarehouseManagerShipmentsQuery>();
        services.AddScoped<IWarehouseManagerShipmentsQuery, WarehouseManagerShipmentsQuery>();

        services.AddScoped<WarehouseManagerTripsQuery>();
        services.AddScoped<IGetWarehouseManagerTripByIdQueryService, WarehouseManagerTripsQuery>();
        services.AddScoped<IGetWarehouseManagerTripsQueryService, WarehouseManagerTripsQuery>();
        services.AddScoped<IWarehouseManagerTripsQuery, WarehouseManagerTripsQuery>();

        return services;
    }
}
