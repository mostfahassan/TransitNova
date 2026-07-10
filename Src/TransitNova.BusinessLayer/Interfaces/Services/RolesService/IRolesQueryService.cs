using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNova.BusinessLayer.Interfaces.Services.RolesService
{
    public interface IRolesQueryService
    {
        Task<IEnumerable<RoleSummaryDto>> GetRolesAsync(CancellationToken cancellationToken);
        Task<RoleMembersDto?> GetRoleByIdAsync(Guid roleId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<RoleMembersDto?> GetRoleMembersAsync(Guid roleId, CancellationToken cancellationToken);
        Task<RoleMembersDto?> GetUsersInRoleAsync(Guid roleId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }

}
