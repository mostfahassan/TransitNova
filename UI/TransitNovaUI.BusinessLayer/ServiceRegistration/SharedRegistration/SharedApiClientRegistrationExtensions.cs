using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Locations.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Notifications;
using TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Warehouses.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Notifications.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Warehouses.Queries;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.SharedRegistration;

public static class SharedApiClientRegistrationExtensions
{
    public static IServiceCollection AddSharedApiClients(this IServiceCollection services)
    {
        services.AddScoped<SharedLocationsQuery>();
        services.AddScoped<IGetCitiesByGovernmentQueryService, SharedLocationsQuery>();
        services.AddScoped<IGetCountriesQueryService, SharedLocationsQuery>();
        services.AddScoped<IGetCountryGovernmentsQueryService, SharedLocationsQuery>();
        services.AddScoped<IGetPublicCitiesQueryService, SharedLocationsQuery>();
        services.AddScoped<IGetPublicCityByIdQueryService, SharedLocationsQuery>();
        services.AddScoped<IGetPublicGovernmentByIdQueryService, SharedLocationsQuery>();
        services.AddScoped<IGetPublicGovernmentsQueryService, SharedLocationsQuery>();

        services.AddScoped<SharedWarehousesQuery>();
        services.AddScoped<IGetSharedWarehousesQueryService, SharedWarehousesQuery>();

        services.AddScoped<SharedNotificationsApi>();
        services.AddScoped<ISharedNotificationsQuery, SharedNotificationsApi>();
        services.AddScoped<ISharedNotificationsCommand, SharedNotificationsApi>();
        services.AddScoped<IGetNotificationsQueryService, SharedNotificationsApi>();
        services.AddScoped<IGetUnreadCountQueryService, SharedNotificationsApi>();
        services.AddScoped<IMarkAllNotificationsReadCommandService, SharedNotificationsApi>();

        return services;
    }
}