namespace TransitNovaUI.BusinessLayer.ApiContracts;

public sealed record ApiErrorItem(string? Code, string Message);

public record ApiResponse
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string? ErrorCode { get; init; }

    public IReadOnlyList<ApiErrorItem>? Errors { get; init; }
}

public sealed record ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }
}
