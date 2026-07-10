using Microsoft.AspNetCore.Identity;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;

namespace TransitNova.InfraStructure.Common.RolesService
{
    internal class RoleAssignmentService(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<AppUser> userManager)
        : IRoleAssignmentService
    {
        public async Task AddUserToRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
        {
            var role = await GetRoleAsync(roleId);
            await AddUserToRoleAsync(userId, role.Name!, ct);
        }
        public async Task RemoveUserFromRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
        {
            var role = await GetRoleAsync(roleId);
            await RemoveUserFromRoleAsync(userId, role.Name!, ct);
        }

        private async Task AddUserToRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            var roleExists = await roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
                throw new KeyNotFoundException($"Role '{roleName}' not found.");

            var isInRole = await userManager.IsInRoleAsync(user, roleName);

            if (isInRole)
                return;

            var result = await userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private async Task RemoveUserFromRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            var roleExists = await roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
                throw new KeyNotFoundException($"Role '{roleName}' not found.");

            var isInRole = await userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
                return;

            var result = await userManager.RemoveFromRoleAsync(user, roleName);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private async Task<IdentityRole<Guid>> GetRoleAsync(Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());

            if (role is null || string.IsNullOrWhiteSpace(role.Name))
                throw new KeyNotFoundException($"Role '{roleId}' not found.");

            return role;
        }
    }
}
