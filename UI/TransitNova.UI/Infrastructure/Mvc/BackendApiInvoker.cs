using TransitNova.Domain.Enums.Result;
using TransitNovaUI.BusinessLayer.ApiContracts;

namespace TransitNova.UI.Infrastructure.Mvc;

public sealed class BackendApiInvoker(IUiAuthSessionService authSession) : IBackendApiInvoker
{
    public async Task<ApiResponse> ExecuteAsync(
        Func<string?, CancellationToken, Task<ApiResponse>> operation,
        bool requiresAuthentication = true,
        CancellationToken cancellationToken = default)
    {
        var token = requiresAuthentication ? authSession.GetAccessToken() : null;
        if (requiresAuthentication && string.IsNullOrWhiteSpace(token))
            return UnauthorizedResponse();

        var response = await operation(token, cancellationToken);
        if (!ShouldRefresh(response))
            return response;

        if (!await authSession.TryRefreshAsync(cancellationToken))
            return response;

        return await operation(authSession.GetAccessToken(), cancellationToken);
    }

    public async Task<ApiResponse<T>> ExecuteAsync<T>(
        Func<string?, CancellationToken, Task<ApiResponse<T>>> operation,
        bool requiresAuthentication = true,
        CancellationToken cancellationToken = default)
    {
        var token = requiresAuthentication ? authSession.GetAccessToken() : null;
        if (requiresAuthentication && string.IsNullOrWhiteSpace(token))
            return UnauthorizedResponse<T>();

        var response = await operation(token, cancellationToken);
        if (!ShouldRefresh(response))
            return response;

        if (!await authSession.TryRefreshAsync(cancellationToken))
            return response;

        return await operation(authSession.GetAccessToken(), cancellationToken);
    }

    private static bool ShouldRefresh(ApiResponse response) =>
        response.Status == ResultStatus.Unauthorized;

    private static ApiResponse UnauthorizedResponse() =>
        new(false, ResultStatus.Unauthorized, new ApiError(ErrorCode.UNAUTHORIZED, "Authentication is required."));

    private static ApiResponse<T> UnauthorizedResponse<T>() =>
        new(default, false, ResultStatus.Unauthorized, new ApiError(ErrorCode.UNAUTHORIZED, "Authentication is required."));
}
