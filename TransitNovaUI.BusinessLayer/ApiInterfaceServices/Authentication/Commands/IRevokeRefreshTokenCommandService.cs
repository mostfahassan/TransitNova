namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IRevokeRefreshTokenCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/refreshToken/{id:guid}";

    Task<ApiResponse> RevokeRefreshTokenAsync(Guid id, CancellationToken cancellationToken = default);
}

