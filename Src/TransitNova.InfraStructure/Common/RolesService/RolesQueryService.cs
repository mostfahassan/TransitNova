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
                    RoleName = r.Name,
                    TotalUsers = context.UserRoles.Count(ur => ur.RoleId == r.Id)
                })
                .ToListAsync(cancellationToken);

        public Task<RoleMembersDto?> GetRoleByIdAsync(Guid roleId, int pageNumber, int pageSize, CancellationToken cancellationToken)
            => GetUsersInRoleAsync(roleId, pageNumber, pageSize, cancellationToken);

        public async Task<RoleMembersDto?> GetRoleMembersAsync(Guid roleId, CancellationToken cancellationToken)
        {
            var role = await roleManager.Roles
                .AsNoTracking()
                .Where(currentRole => currentRole.Id == roleId)
                .Select(currentRole => new
                {
                    currentRole.Id,
                    currentRole.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (role is null)
                return null;

            var utcNow = DateTimeOffset.UtcNow;

            var members = await context.AppUsers
                .AsNoTracking()
                .OrderBy(user => user.FullName ?? user.UserName)
                .Select(user => new RoleMemberDto
                {
                    UserId = user.Id,
                    FullName = user.FullName ?? user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType.ToString(),
                    Status = user.LockoutEnd.HasValue && user.LockoutEnd > utcNow ? "Locked" : "Active",
                    IsInRole = context.UserRoles.Any(userRole =>
                        userRole.RoleId == roleId &&
                        userRole.UserId == user.Id)
                })
                .ToListAsync(cancellationToken);

            return new RoleMembersDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                TotalUsers = members.Count,
                PageNumber = 1,
                PageSize = Math.Max(1, members.Count),
                Users = members
            };
        }

        public async Task<RoleMembersDto?> GetUsersInRoleAsync(Guid roleId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var role = await roleManager.Roles
                .AsNoTracking()
                .Where(currentRole => currentRole.Id == roleId)
                .Select(currentRole => new
                {
                    currentRole.Id,
                    currentRole.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (role is null)
                return null;

            var utcNow = DateTimeOffset.UtcNow;

            var membersQuery =
                from userRole in context.UserRoles.AsNoTracking()
                where userRole.RoleId == roleId
                join user in context.AppUsers.AsNoTracking() on userRole.UserId equals user.Id
                orderby user.FullName ?? user.UserName
                select new RoleMemberDto
                {
                    UserId = user.Id,
                    FullName = user.FullName ?? user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType.ToString(),
                    Status = user.LockoutEnd.HasValue && user.LockoutEnd > utcNow ? "Locked" : "Active",
                    IsInRole = true
                };

            var totalUsers = await membersQuery.CountAsync(cancellationToken);
            var members = await membersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new RoleMembersDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                TotalUsers = totalUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Users = members
            };
        }
    }
}
