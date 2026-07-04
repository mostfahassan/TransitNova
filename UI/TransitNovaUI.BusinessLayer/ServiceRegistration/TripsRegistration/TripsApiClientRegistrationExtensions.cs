using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Trips.Carriers.Query;
using TransitNovaUI.BusinessLayer.ApiImplementation.Trips.OperationManager.Command;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Segregations.Commands;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.TripsRegistration;

public static class TripsApiClientRegistrationExtensions
{
    public static IServiceCollection AddTripsApiClients(this IServiceCollection services)
    {
        services.AddScoped<TripsCarrierTripsQuery>();
        services.AddScoped<ICarrierTripsQuery, TripsCarrierTripsQuery>();
        services.AddScoped<ITripsCarrierTripsQuery, TripsCarrierTripsQuery>();
        services.AddScoped<IGetCarrierTripByIdQueryService, TripsCarrierTripsQuery>();
        services.AddScoped<IGetCarrierTripsQueryService, TripsCarrierTripsQuery>();

        services.AddScoped<TripsOperationManagerTripsCommand>();
        services.AddScoped<IOperationManagerTripsCommand, TripsOperationManagerTripsCommand>();
        services.AddScoped<ITripsOperationManagerTripsCommand, TripsOperationManagerTripsCommand>();
        services.AddScoped<IStartDeliveryTripCommandService, TripsOperationManagerTripsCommand>();
        services.AddScoped<IStartPickupTripCommandService, TripsOperationManagerTripsCommand>();

        return services;
    }
}
