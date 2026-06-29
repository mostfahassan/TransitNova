using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository;
namespace TransitNova.Api.AuthorizationResource.Handler
{
    public class IsWarehouseManagerRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        IWarehouseManagerRuleseRepository queryRepository)
        : AuthorizationHandler<IsWarehouseManagerRequirement, Guid>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsWarehouseManagerRequirement requirement, Guid warehouseId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var currentUserId))
                return;

            var isWarehouseManager = await queryRepository.IsWarehouseManager(currentUserId, warehouseId, CancellationToken.None);
            if (isWarehouseManager) context.Succeed(requirement);
        }
    }
}
