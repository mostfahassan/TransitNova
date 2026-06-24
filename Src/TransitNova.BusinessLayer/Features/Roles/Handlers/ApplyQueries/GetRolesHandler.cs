using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Roles;
using TransitNova.BusinessLayer.Features.Roles.Queries;
using TransitNova.BusinessLayer.Interfaces.Services.RolesService;

namespace TransitNova.BusinessLayer.Features.Roles.Handlers.ApplyQueries
{
    public sealed class GetRolesHandler(
        IRolesQueryService rolesQueryService,
        ILogger<GetRolesHandler> logger)
        : IQueryHandler<GetRolesQuery, Result<List<RoleSummaryDto>>>
    {
        public async Task<Result<List<RoleSummaryDto>>> Handle(GetRolesQuery request, CancellationToken ct)
        {
            logger.LogInformation("Retrieving roles list");

            var roles = (await rolesQueryService.GetRolesAsync(ct)).ToList();

            logger.LogInformation("Retrieved {RolesCount} roles", roles.Count);
            return Result<List<RoleSummaryDto>>.Success(roles);
        }
    }
}
