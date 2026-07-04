using TransitNovaUI.BusinessLayer.ApiContracts;

namespace TransitNova.UI.Infrastructure.Mvc;

public interface IBackendApiInvoker
{
    Task<ApiResponse> ExecuteAsync(
        Func<string?, CancellationToken, Task<ApiResponse>> operation,
        bool requiresAuthentication = true,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<T>> ExecuteAsync<T>(
        Func<string?, CancellationToken, Task<ApiResponse<T>>> operation,
        bool requiresAuthentication = true,
        CancellationToken cancellationToken = default);
}
