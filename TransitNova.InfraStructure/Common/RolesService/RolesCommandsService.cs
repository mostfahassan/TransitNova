using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
namespace TransitNova.InfraStructure.Common.RolesService
{
    internal sealed class RolesCommandsService(RoleManager<IdentityRole<Guid>> roleManager, UserManager<AppUser> userManager, ILogger<RolesCommandsService> logger)
        : IRolesCommandsService
    {
        public async Task AddNewRole(string roleName, CancellationToken cancellationToken = default)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                throw new InvalidOperationException($"Role '{roleName}' already exists.");

            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task DeleteRole(Guid roleId, CancellationToken cancellationToken = default)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
                throw new KeyNotFoundException($"Role '{roleId}' not found.");

            var result = await roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateRole(Guid roleId, string newRoleName, CancellationToken cancellationToken = default)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
                throw new KeyNotFoundException($"Role '{roleId}' not found.");

            role.Name = newRoleName;
            role.NormalizedName = newRoleName.ToUpperInvariant();

            var result = await roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateRoleMembersAsync(Guid roleId, IReadOnlyCollection<RoleMemberUpdateDto> users, CancellationToken cancellationToken = default)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
                throw new KeyNotFoundException($"Role '{roleId}' not found.");

            if (string.IsNullOrWhiteSpace(role.Name))
                throw new InvalidOperationException($"Role '{roleId}' has no valid name.");

            // To Avoid Repeatness of user roles Ex => 1 : manager  , 1 : carrier i will take the last 1 that its role will be carrier 
            var desiredMembers = users
                .GroupBy(user => user.UserId)
                .Select(group => group.Last())
                .ToList();

            // convert User Ids into Hash set to be fast search for each User 
            var requestedUserIds = desiredMembers
                .Select(user => user.UserId)
                .ToHashSet();

            // load all usersIds that are being saved in Hash set (why i use hashset ) 
            var loadedUsers = await userManager.Users
                .Where(user => requestedUserIds.Contains(user.Id))
                .ToDictionaryAsync(user => user.Id, cancellationToken);


            // checking if there is a missing user Id ?
            var missingUsers = requestedUserIds
                .Where(userId => !loadedUsers.ContainsKey(userId))
                .ToList();

            // if there is a missing member all process will be failed 
            if (missingUsers.Count > 0)
                throw new KeyNotFoundException($"Users not found: {string.Join(", ", missingUsers)}");
            // here i saved (N+1) query and avoid using Find Async for each user 
            var currentRoleUsers = await userManager.GetUsersInRoleAsync(role.Name);
            var currentRoleUserIds = currentRoleUsers
                .Select(user => user.Id)
                .ToHashSet();

            logger.LogInformation("Synchronizing role members. RoleId: {RoleId}, UsersCount: {UsersCount}", roleId, desiredMembers.Count);

            foreach (var desiredMember in desiredMembers)
            {
                // if user cancel process or close the tap to save and optimize resources
                cancellationToken.ThrowIfCancellationRequested();

                // Updateing User Role 
                var isCurrentlyInRole = currentRoleUserIds.Contains(desiredMember.UserId);
                var user = loadedUsers[desiredMember.UserId];

                if (desiredMember.IsInRole && !isCurrentlyInRole)
                {
                    var addResult = await userManager.AddToRoleAsync(user, role.Name);

                    if (!addResult.Succeeded)
                        throw new InvalidOperationException(string.Join(", ", addResult.Errors.Select(e => e.Description)));

                    logger.LogInformation(
                        "User added to role. RoleId: {RoleId}, UserId: {UserId}",
                        roleId,
                        desiredMember.UserId);
                }

                if (!desiredMember.IsInRole && isCurrentlyInRole)
                {
                    var removeResult = await userManager.RemoveFromRoleAsync(user, role.Name);

                    if (!removeResult.Succeeded)
                        throw new InvalidOperationException(string.Join(", ", removeResult.Errors.Select(e => e.Description)));

                    logger.LogInformation("User removed from role. RoleId: {RoleId}, UserId: {UserId}", roleId, desiredMember.UserId);
                }
            }
            logger.LogInformation("Role members synchronized successfully. RoleId: {RoleId}, UsersCount: {UsersCount}", roleId, desiredMembers.Count);
        }
    }
}
