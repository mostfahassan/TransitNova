using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.TokenRepository;
using TransitNova.BusinessLayer.Interfaces.Token;
using TransitNova.InfraStructure.Repository.TokenRepository;
using TransitNova.InfraStructure.Token;
namespace TransitNova.InfraStructure.ServiceRegistration.TokenRegistration
{
    public static class TokenRegistrationExtensions
    {
        public static IServiceCollection AddTokenServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenProvider, TokenGenerator>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }
    }
}
