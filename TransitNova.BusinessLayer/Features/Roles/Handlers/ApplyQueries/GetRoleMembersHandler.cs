using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.BusinessLayer.Features.Roles.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyQueries
{
    public sealed class GetRoleMembersHandler(
        IRolesQueryService rolesQueryService,
        ILogger<GetRoleMembersHandler> logger)
        : IQueryHandler<GetRoleMembersQuery, Result<RoleMembersDto>>
    {
        public async Task<Result<RoleMembersDto>> Handle(GetRoleMembersQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving role members. RoleId: {RoleId}", request.RoleId);

            var roleMembers = await rolesQueryService.GetRoleMembersAsync(request.RoleId, ct);
            if (roleMembers is null)
            {
                logger.LogWarning("Role members not found because role does not exist. RoleId: {RoleId}", request.RoleId);
                return Result<RoleMembersDto>.NotFound(Errors.NotFound("Role not found."));
            }

            logger.LogInformation(
                "Role members retrieved successfully. RoleId: {RoleId}, UsersCount: {UsersCount}",
                request.RoleId,
                roleMembers.TotalUsers);

            return Result<RoleMembersDto>.Success(roleMembers);
        }
    }
}
