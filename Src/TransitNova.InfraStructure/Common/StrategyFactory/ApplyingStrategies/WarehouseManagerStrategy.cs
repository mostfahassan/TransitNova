using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Interfaces.Services.Users.RegistrationStrategy;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Users;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Common.StrategyFactory.ApplyingStrategies
{
    public class WarehouseManagerStrategy(AppDbContext context) : IUserRegistrationStrategy
    {
        public UserType UserType => UserType.WarehouseManager;

        public async Task ExecuteAsync(Guid userId, RegisterDto dto, CancellationToken cancellationToken)
        {
            var warehouseManager = WarehouseManagerProfile.Create(userId, dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber, dto.Address, dto.CityId);
            await context.WarehouseManagersProfiles.AddAsync(warehouseManager, cancellationToken);
        }
    }
}