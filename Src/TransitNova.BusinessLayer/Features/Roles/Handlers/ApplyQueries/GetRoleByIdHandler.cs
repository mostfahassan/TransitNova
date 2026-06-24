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
        : IQueryHandler<GetRoleByIdQuery, Result<RoleSummaryDto>>
    {
        public async Task<Result<RoleSummaryDto>> Handle(GetRoleByIdQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving role details. RoleId: {RoleId}", request.RoleId);

            var role = await rolesQueryService.GetRoleByIdAsync(request.RoleId, ct);
            if (role is null)
            {
                logger.LogWarning("Role details not found. RoleId: {RoleId}", request.RoleId);
                return Result<RoleSummaryDto>.NotFound(Errors.NotFound("Role not found."));
            }

            logger.LogInformation("Role details retrieved successfully. RoleId: {RoleId}", request.RoleId);
            return Result<RoleSummaryDto>.Success(role);
        }
    }
}
