using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.BusinessLayer.Features.Roles.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyQueries
{
    public sealed class GetRoleByIdHandler(
        IRolesQueryService rolesQueryService,
        ILogger<GetRoleByIdHandler> logger)
        : IQueryHandler<GetRoleByIdQuery, Result<RoleMembersDto>>
    {
        public async Task<Result<RoleMembersDto>> Handle(GetRoleByIdQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving role details. RoleId: {RoleId}, PageNumber: {PageNumber}, PageSize: {PageSize}", request.RoleId, request.PageNumber, request.PageSize);

            var role = await rolesQueryService.GetRoleByIdAsync(request.RoleId, request.PageNumber, request.PageSize, ct);
            if (role is null)
            {
                logger.LogWarning("Role details not found. RoleId: {RoleId}", request.RoleId);
                return Result<RoleMembersDto>.NotFound(Errors.NotFound("Role not found."));
            }

            logger.LogInformation("Role details retrieved successfully. RoleId: {RoleId}, TotalUsers: {TotalUsers}", request.RoleId, role.TotalUsers);
            return Result<RoleMembersDto>.Success(role);
        }
    }
}
