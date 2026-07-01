namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRoleByIdQueryService
{
    Task<ApiResponse<UiRoleSummaryDto>> GetRoleByIdAsync(Guid roleId, string bearerToken, CancellationToken cancellationToken = default);
}

