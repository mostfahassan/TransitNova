using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ServiceRegistration.AdminRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.AuthenticationRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.CarrierRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.OperationManagerRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.SharedRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.ShipmentRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.TripsRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.UserRegistration;
using TransitNovaUI.BusinessLayer.ServiceRegistration.WarehouseManagerRegistration;

namespace TransitNovaUI.BusinessLayer
{
    public static class Dependencies
    {
        public static IServiceCollection AddTransitNovaApiClients(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<System.Net.Http.HttpClient>(serviceProvider => serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient());
            services.AddScoped<IHttpHandler, HttpHandler>();

            services
                .AddAdminApiClients()
                .AddAuthenticationApiClients()
                .AddCarrierApiClients()
                .AddOperationManagerApiClients()
                .AddSharedApiClients()
                .AddShipmentApiClients()
                .AddUserApiClients()
                .AddWarehouseManagerApiClients()
                .AddTripsApiClients();

            return services;
        }
    }
}