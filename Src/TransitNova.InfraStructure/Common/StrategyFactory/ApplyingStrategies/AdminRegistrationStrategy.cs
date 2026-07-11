
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Users;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Common.StrategyFactory.ApplyingStrategies
{
    public class AdminRegistrationStrategy(AppDbContext context) : IUserRegistrationStrategy
    {
        public UserType UserType => UserType.Admin;
        public async Task ExecuteAsync(Guid userId, RegisterDto dto, CancellationToken cancellationToken)
        {
            var admin = AdminProfile.Create(userId, dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber, dto.Address.ToDomain(), dto.CityId);
            await context.Admins.AddAsync(admin, cancellationToken);
        }
    }
}
