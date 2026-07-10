namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRoleByIdQueryService
{
    Task<ApiResponse<UiRoleMembersDto>> GetRoleByIdAsync(Guid roleId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
