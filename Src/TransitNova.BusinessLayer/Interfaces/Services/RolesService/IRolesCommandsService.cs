
using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNova.BusinessLayer.Interfaces.Services.RolesService
{
    public interface IRolesCommandsService
    {
        Task AddNewRoleAsync(string roleName,CancellationToken cancellationToken =default);
        Task DeleteRoleAsync(Guid roleId,CancellationToken cancellationToken =default);
        Task UpdateRoleAsync(Guid roleId, string newRoleName, CancellationToken cancellationToken = default);
        Task UpdateRoleMembersAsync(Guid roleId, IReadOnlyCollection<RoleMemberUpdateDto> users, CancellationToken cancellationToken = default);
    }

}
