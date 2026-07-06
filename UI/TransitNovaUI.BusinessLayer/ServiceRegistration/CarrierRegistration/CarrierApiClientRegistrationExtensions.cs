using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Analytics.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Dashboard.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Profile.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Profile.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Shipments.Command;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Shipments.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Vehicles.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Vehicles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Vehicles.Segregation;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.CarrierRegistration;

public static class CarrierApiClientRegistrationExtensions
{
    public static IServiceCollection AddCarrierApiClients(this IServiceCollection services)
    {
        services.AddScoped<CarrierAnalyticsQuery>();
        services.AddScoped<ICarrierAnalyticsQuery, CarrierAnalyticsQuery>();
        services.AddScoped<ICarrierAnalyticalQuery, CarrierAnalyticsQuery>();
        services.AddScoped<IGetCarrierRatingQueryService, CarrierAnalyticsQuery>();
        services.AddScoped<IGetCarrierRevenueQueryService, CarrierAnalyticsQuery>();

        services.AddScoped<CarrierDashboardQuery>();
        services.AddScoped<ICarrierDashboardQuery, CarrierDashboardQuery>();
        services.AddScoped<IGetCarrierDashboardQueryService, CarrierDashboardQuery>();

        services.AddScoped<CarrierProfileCommand>();
        services.AddScoped<ICarrierProfileCommand, CarrierProfileCommand>();
        services.AddScoped<IAddCarrierAdditionalInfoCommandService, CarrierProfileCommand>();
        services.AddScoped<IUpdateCarrierProfileCommandService, CarrierProfileCommand>();

        services.AddScoped<CarrierProfileQuery>();
        services.AddScoped<ICarrierProfileQuery, CarrierProfileQuery>();
        services.AddScoped<IGetCarrierProfileQueryService, CarrierProfileQuery>();

        services.AddScoped<CarrierVehiclesQuery>();
        services.AddScoped<ICarrierVehiclesQuery, CarrierVehiclesQuery>();
        services.AddScoped<IGetCarrierVehicleQueryService, CarrierVehiclesQuery>();

        services.AddScoped<CarrierShipmentsCommand>();
        services.AddScoped<ICarrierShipmentsCommand, CarrierShipmentsCommand>();
        services.AddScoped<ICarrierShipmentCommand, CarrierShipmentsCommand>();
        services.AddScoped<ICompleteDeliveryCommandService, CarrierShipmentsCommand>();
        services.AddScoped<ICompletePickupCommandService, CarrierShipmentsCommand>();
        services.AddScoped<IMarkShipmentPickedUpCommandService, CarrierShipmentsCommand>();
        services.AddScoped<IUpdateCarrierStatusCommandService, CarrierShipmentsCommand>();

        services.AddScoped<CarrierShipmentsQuery>();
        services.AddScoped<ICarrierShipmentsQuery, CarrierShipmentsQuery>();
        services.AddScoped<ICarrierShipmentQuery, CarrierShipmentsQuery>();
        services.AddScoped<IGetCarrierShipmentByIdQueryService, CarrierShipmentsQuery>();
        services.AddScoped<IGetCarrierShipmentsQueryService, CarrierShipmentsQuery>();

        return services;
    }
}
