namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IRevokeRefreshTokenCommandService
{
    Task<ApiResponse> RevokeRefreshTokenAsync(Guid userId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

