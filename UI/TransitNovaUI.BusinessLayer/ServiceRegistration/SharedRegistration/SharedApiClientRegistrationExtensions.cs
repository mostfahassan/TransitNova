using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Locations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

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

        return services;
    }
}
