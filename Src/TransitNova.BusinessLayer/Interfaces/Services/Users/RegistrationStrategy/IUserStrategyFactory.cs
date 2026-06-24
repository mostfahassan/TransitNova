using TransitNova.Domain.Enums.Users;

namespace TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy
{
    public interface IUserStrategyFactory
    {
        public IUserRegistrationStrategy? ResolveUserStrategy(UserType userType);   
    }
}
