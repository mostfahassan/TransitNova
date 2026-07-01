namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface IUpdateRoleCommandService
{
    Task<ApiResponse> UpdateRoleAsync(Guid roleId, UiRoleNameDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

