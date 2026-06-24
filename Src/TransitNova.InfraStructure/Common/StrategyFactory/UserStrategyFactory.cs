
using TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.InfraStructure.Common.StrategyFactory
{
    public class UserStrategyFactory : IUserStrategyFactory
    {
        private readonly IEnumerable<IUserRegistrationStrategy> _strategies;
        public UserStrategyFactory(IEnumerable<IUserRegistrationStrategy> strategies)
        => _strategies = strategies;
        public IUserRegistrationStrategy? ResolveUserStrategy(UserType userType)
        {
            var strategy = _strategies.FirstOrDefault(s => s.UserType == userType);
            return strategy;
        }
    }
}
