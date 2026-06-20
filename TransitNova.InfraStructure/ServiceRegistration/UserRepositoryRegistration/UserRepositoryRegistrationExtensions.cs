using Microsoft.Extensions.DependencyInjection;
using TransitNova.BusinessLayer.Interfaces.Repositories.Idempotent;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.InfraStructure.Common.IdentityService;
using TransitNova.InfraStructure.Repository.Idempotent;
using TransitNova.InfraStructure.Repository.User;
namespace TransitNova.InfraStructure.ServiceRegistration.UserRepositoryRegistration
{
    public static class UserRepositoryRegistrationExtensions
    {
        public static IServiceCollection AddUserRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserQueryRepository, UserQueryRepository>()
                    .AddScoped<IUserAuthQueryService, UserAuthQueryService>()
                    .AddScoped<IUserAuthCommandsService, UserAuthCommandsService>()
                    .AddScoped<IUserAuthRulesService, UserAuthRulesService>()
                    .AddScoped<IReceiverRepository, ReceiverRepository>()
                    .AddScoped<IUserRulesRepository, UserRulesRepository>()
                    .AddScoped<IIdempotentRepository, IdetmpoentRepository>();
            return services;
        }
    }
}
