
using TransitNova.BusinessLayer.DTOs.Roles;
namespace TransitNova.BusinessLayer.Interfaces.Services.RolesService
{
    public interface IRoleAssignmentService
    {
        Task AddUserToRoleAsync(Guid userId, Guid roleId, CancellationToken ct);

        Task RemoveUserFromRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    }

}
