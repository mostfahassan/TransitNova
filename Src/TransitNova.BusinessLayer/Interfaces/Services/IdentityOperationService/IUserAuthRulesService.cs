
using TransitNova.BusinessLayer.DTOs.AppUser;

namespace TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService
{
    public interface IUserAuthRulesService
    {
        Task<bool> IsUserLockedAsync(AppUserDto user, CancellationToken ct);
        Task<(bool, AppUserDto?, string)> ValidatePasswordAsync(string userName, string password, bool lockoutOnFailure, CancellationToken ct);
    }
}

