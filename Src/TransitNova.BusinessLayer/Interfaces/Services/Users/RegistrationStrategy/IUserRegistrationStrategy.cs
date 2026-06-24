
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy
{
    public interface IUserRegistrationStrategy
    {
        UserType UserType { get; }
        public Task ExecuteAsync(
         Guid userId,
         RegisterDto dto,
         CancellationToken cancellationToken);
    }
}
