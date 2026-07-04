using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using TransitNova.BusinessLayer.Common.ResultPattern;
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
    public ApiError? Error { get; }
    public IReadOnlyList<ApiError> Errors { get; }


    [JsonConstructor]
    public ApiResponse(bool isSuccess, ResultStatus status, ApiError? error = null, IReadOnlyList<ApiError>? errors = null, string? message = null)
    {
        IsSuccess = isSuccess;
        Status = status;
        Message = message;
        Error = error;
        Errors = errors?.ToList() ?? [];
    }
    public static ApiResponse Failure(ApiError error)
        => new(false, ResultStatus.Failure, error);

}



public sealed record ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }

    public ApiResponse(T? data, bool isSuccess, ResultStatus status, ApiError? error = null, IReadOnlyList<ApiError>? errors = null, string? message = null)
         : base(isSuccess, status, error, errors, message)
    {
        Data = data;
    }
    public  static ApiResponse<T> FailedResponse(ApiError error)
       => new(default , false, ResultStatus.Failure, error);


}
