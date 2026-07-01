namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRolesQueryService
{
    Task<ApiResponse<List<UiRoleSummaryDto>>> GetRolesAsync(string bearerToken, CancellationToken cancellationToken = default);
}

