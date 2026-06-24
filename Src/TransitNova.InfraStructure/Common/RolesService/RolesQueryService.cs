using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Common.RolesService
{
    internal class RolesQueryService(
        RoleManager<IdentityRole<Guid>> roleManager,
        AppDbContext context)
        : IRolesQueryService
    {
        public async Task<IEnumerable<RoleSummaryDto>> GetRolesAsync(CancellationToken cancellationToken = default)
            => await roleManager.Roles
                .AsNoTracking()
                .Select(r => new RoleSummaryDto
                {
                    RoleId = r.Id.ToString(),
                    RoleName = r.Name
                })
                .ToListAsync(cancellationToken);

        public async Task<RoleSummaryDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken)
            => await roleManager.Roles
                .AsNoTracking()
                .Where(role => role.Id == roleId)
                .Select(role => new RoleSummaryDto
                {
                    RoleId = role.Id.ToString(),
                    RoleName = role.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<RoleMembersDto?> GetRoleMembersAsync(Guid roleId, CancellationToken cancellationToken)
        {
            var role = await roleManager.Roles
                .AsNoTracking()
                .Where(role => role.Id == roleId)
                .Select(role => new
                {
                    role.Id,
                    role.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (role is null)
                return null;

            var users = await context.AppUsers
                .AsNoTracking()
                .OrderBy(user => user.UserName)
                .Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.UserType,
                    IsInRole = context.UserRoles.Any(userRole =>
                        userRole.RoleId == roleId &&
                        userRole.UserId == user.Id),
                    UserFullName = context.UserProfiles
                        .Where(profile => profile.AppUserId == user.Id)
                        .Select(profile => profile.FullName)
                        .FirstOrDefault(),
                    CarrierFullName = context.Carriers
                        .Where(profile => profile.AppUserId == user.Id)
                        .Select(profile => profile.FullName)
                        .FirstOrDefault(),
                    OperationManagerFullName = context.OperationManagerProfiles
                        .Where(profile => profile.AppUserId == user.Id)
                        .Select(profile => profile.FullName)
                        .FirstOrDefault(),
                    AdminFullName = context.Admins
                        .Where(profile => profile.AppUserId == user.Id)
                        .Select(profile => profile.FullName)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var members = users
                .Select(user => new RoleMemberDto
                {
                    UserId = user.Id,
                    FullName = user.UserFullName ??
                               user.CarrierFullName ??
                               user.OperationManagerFullName ??
                               user.AdminFullName ??
                               user.UserName ??
                               string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType.ToString(),
                    IsInRole = user.IsInRole
                })
                .ToList();

            return new RoleMembersDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                TotalUsers = members.Count,
                Users = members
            };
        }

        public async Task<IEnumerable<RoleDetailsDto>> GetUsersInRoleAsync(CancellationToken cancellationToken)
        {
            var roles = await roleManager.Roles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = new List<RoleDetailsDto>();

            foreach (var role in roles)
            {
                var roleMembers = await GetRoleMembersAsync(role.Id, cancellationToken);
                var usersInRole = roleMembers?.Users
                    .Where(user => user.IsInRole)
                    .Select(user => new RoleUserDto
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        UserType = user.UserType
                    })
                    .ToList() ?? [];

                result.Add(new RoleDetailsDto
                {
                    RoleId = role.Id.ToString(),
                    RoleName = role.Name ?? string.Empty,
                    UsersCount = usersInRole.Count,
                    Users = usersInRole
                });
            }

            return result;
        }
    }
}
