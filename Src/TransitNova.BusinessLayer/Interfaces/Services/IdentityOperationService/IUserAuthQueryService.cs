
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
namespace TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService
{
    public interface IUserAuthQueryService
    {
        Task<AppUserDto?> FindByIdAsync(Guid userId, CancellationToken ct);
        Task<AppUserDto?> FindByNameAsync(string userName, CancellationToken ct);
        Task<AppUserDto?> FindByEmailAsync(string email, CancellationToken ct);
        Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken ct);
        
    }
}
