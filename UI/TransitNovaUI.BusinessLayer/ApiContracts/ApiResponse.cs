using System.Text.Json.Serialization;
using TransitNova.Domain.Enums.Result;

namespace TransitNovaUI.BusinessLayer.ApiContracts;

public sealed record ApiError(ErrorCode? Code, string Message);

public record ApiResponse
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ResultStatus Status { get; }
    public string? Message { get; init; }
    public int StatusCode => (int)Status;
    public ErrorCode? ErrorCode { get; }
    public ApiError? Error { get; }
    public IReadOnlyList<ApiError> Errors { get; }

    [JsonConstructor]
    public ApiResponse(bool isSuccess, ResultStatus status, int statusCode, ErrorCode? errorCode = null, ApiError? error = null, IReadOnlyList<ApiError>? errors = null, string? message = null)
    {
        IsSuccess = isSuccess;
        Status = status != default ? status : ResolveStatus(statusCode, isSuccess);
        Message = message;
        ErrorCode = errorCode ?? error?.Code;
        Error = error ?? CreateError(ErrorCode, message);
        Errors = errors?.ToList() ?? [];
    }

    public ApiResponse(bool isSuccess, ResultStatus status, ApiError? error = null, IReadOnlyList<ApiError>? errors = null, string? message = null)
        : this(isSuccess, status, (int)status, error?.Code, error, errors, message)
    {
    }

    public static ApiResponse Failure(ApiError error) =>
        new(false, ResultStatus.Failure, error);

    protected static ResultStatus ResolveStatus(int statusCode, bool isSuccess)
    {
        return statusCode switch
        {
            200 => ResultStatus.Success,
            201 => ResultStatus.Created,
            204 => ResultStatus.NoContent,
            400 => ResultStatus.Failure,
            401 => ResultStatus.Unauthorized,
            403 => ResultStatus.Forbidden,
            404 => ResultStatus.NotFound,
            409 => ResultStatus.Conflict,
            422 => ResultStatus.ValidationError,
            500 => ResultStatus.UnExpected,
            _ => isSuccess ? ResultStatus.Success : ResultStatus.Failure
        };
    }

    private static ApiError? CreateError(ErrorCode? errorCode, string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        return new ApiError(errorCode ?? TransitNova.Domain.Enums.Result.ErrorCode.FAILED, message);
    }
}

public sealed record ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }

    [JsonConstructor]
    public ApiResponse(T? data, bool isSuccess, ResultStatus status, int statusCode, ErrorCode? errorCode = null, ApiError? error = null, IReadOnlyList<ApiError>? errors = null, string? message = null)
        : base(isSuccess, status, statusCode, errorCode, error, errors, message)
    {
        Data = data;
    }

    public ApiResponse(T? data, bool isSuccess, ResultStatus status, ApiError? error = null, IReadOnlyList<ApiError>? errors = null, string? message = null)
        : this(data, isSuccess, status, (int)status, error?.Code, error, errors, message)
    {
    }

    public static ApiResponse<T> FailedResponse(ApiError error) =>
        new(default, false, ResultStatus.Failure, error);
}