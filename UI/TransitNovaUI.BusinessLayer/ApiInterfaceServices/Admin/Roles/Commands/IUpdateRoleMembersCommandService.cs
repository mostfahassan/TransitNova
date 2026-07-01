namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface IUpdateRoleMembersCommandService
{
    Task<ApiResponse> UpdateRoleMembersAsync(Guid roleId, UiUpdateRoleMembersDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

