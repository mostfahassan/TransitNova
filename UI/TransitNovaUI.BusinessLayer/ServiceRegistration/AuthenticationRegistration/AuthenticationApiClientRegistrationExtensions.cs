using Microsoft.Extensions.DependencyInjection;
using TransitNovaUI.BusinessLayer.ApiImplementation.Authentication.Command;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation;

namespace TransitNovaUI.BusinessLayer.ServiceRegistration.AuthenticationRegistration;

public static class AuthenticationApiClientRegistrationExtensions
{
    public static IServiceCollection AddAuthenticationApiClients(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationCommand>();
        services.AddScoped<IAuthenticationCommand, AuthenticationCommand>();
        services.AddScoped<IChangePasswordCommandService, AuthenticationCommand>();
        services.AddScoped<ILoginCommandService, AuthenticationCommand>();
        services.AddScoped<IRefreshTokenCommandService, AuthenticationCommand>();
        services.AddScoped<IRegisterCommandService, AuthenticationCommand>();
        services.AddScoped<IRevokeRefreshTokenCommandService, AuthenticationCommand>();
        services.AddScoped<ISignOutCommandService, AuthenticationCommand>();

        return services;
    }
}
