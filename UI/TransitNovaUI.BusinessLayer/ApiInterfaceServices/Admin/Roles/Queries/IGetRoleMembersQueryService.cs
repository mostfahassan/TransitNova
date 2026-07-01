namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRoleMembersQueryService
{
    Task<ApiResponse<UiRoleMembersDto>> GetRoleMembersAsync(Guid roleId, string bearerToken, CancellationToken cancellationToken = default);
}

