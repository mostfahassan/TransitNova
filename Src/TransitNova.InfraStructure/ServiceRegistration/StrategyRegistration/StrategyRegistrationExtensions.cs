using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy;
using TransitNova.InfraStructure.Common.StrategyFactory;
using TransitNova.InfraStructure.Common.StrategyFactory.ApplyingStrategies;

namespace TransitNova.InfraStructure.ServiceRegistration.StrategyRegistration
{
    public static class StrategyRegistrationExtensions
    {
        public static IServiceCollection AddRegistrationStrategies(this IServiceCollection services)
        {
            services.AddScoped<IUserRegistrationStrategy, UserRegistrationStrategy>()
                    .AddScoped<IUserRegistrationStrategy, CarrierRegistrationStrategy>()
                    .AddScoped<IUserRegistrationStrategy, OperationManagerRegistrationStrategy>()
                    .AddScoped<IUserRegistrationStrategy, AdminRegistrationStrategy>()
                    .AddScoped<IUserStrategyFactory, UserStrategyFactory>();

            return services;
        }
    }
}
