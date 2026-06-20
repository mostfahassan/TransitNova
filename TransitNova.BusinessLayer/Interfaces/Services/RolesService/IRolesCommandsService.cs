
using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNova.BusinessLayer.Interfaces.Services.RolesService
{
    public interface IRolesCommandsService
    {
        Task AddNewRole(string roleName,CancellationToken cancellationToken =default);
        Task DeleteRole(Guid roleId,CancellationToken cancellationToken =default);
        Task UpdateRole(Guid roleId, string newRoleName, CancellationToken cancellationToken = default);
        Task UpdateRoleMembersAsync(Guid roleId, IReadOnlyCollection<RoleMemberUpdateDto> users, CancellationToken cancellationToken = default);
    }
    public interface IRolesQueryService
    {
        Task<IEnumerable<RoleSummaryDto>> GetRolesAsync(CancellationToken cancellationToken );
        Task<RoleSummaryDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
        Task<RoleMembersDto?> GetRoleMembersAsync(Guid roleId, CancellationToken cancellationToken);
        Task<IEnumerable<RoleDetailsDto>> GetUsersInRoleAsync(CancellationToken cancellationToken);
    }
    public interface IRoleAssignmentService
    {
        Task AddUserToRoleAsync(Guid userId, Guid roleId, CancellationToken ct);

        Task RemoveUserFromRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    }
}
