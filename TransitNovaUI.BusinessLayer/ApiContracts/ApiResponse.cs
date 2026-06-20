namespace TransitNovaUI.BusinessLayer.ApiContracts;

public record ApiResponse
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public int StatusCode { get; init; }

    public string? ErrorCode { get; init; }

    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }
}

public sealed record ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }
}
