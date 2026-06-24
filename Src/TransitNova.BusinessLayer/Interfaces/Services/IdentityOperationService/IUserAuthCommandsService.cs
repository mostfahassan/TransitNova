
using TransitNova.BusinessLayer.DTOs.AppUser;

namespace TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService
{
    public interface IUserAuthCommandsService
    {
        Task<AppUserDto> CreateUserAsync(AppUserDto userDto, string password, CancellationToken ct);
        Task AddToRoleAsync(AppUserDto userDto, string roleName, CancellationToken ct);
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct);
        Task DeleteUserAsync(AppUserDto userDto, CancellationToken ct);
    }
}
