
using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNova.BusinessLayer.Interfaces.Services.RolesService
{
    public interface IRolesQueryService
    {
        Task<IEnumerable<RoleSummaryDto>> GetRolesAsync(CancellationToken cancellationToken );
        Task<RoleSummaryDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
        Task<RoleMembersDto?> GetRoleMembersAsync(Guid roleId, CancellationToken cancellationToken);
        Task<IEnumerable<RoleDetailsDto>> GetUsersInRoleAsync(CancellationToken cancellationToken);
    }

}
