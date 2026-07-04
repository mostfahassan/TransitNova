using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Shipments.Command;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Pricing.Segregation;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.ShipmentRegistration;

public static class ShipmentApiClientRegistrationExtensions
{
    public static IServiceCollection AddShipmentApiClients(this IServiceCollection services)
    {
        services.AddScoped<ShipmentRateCalculationCommand>();
        services.AddScoped<IUserPricingCommand, ShipmentRateCalculationCommand>();
        services.AddScoped<IRateCalculationCommandService, ShipmentRateCalculationCommand>();

        return services;
    }
}
