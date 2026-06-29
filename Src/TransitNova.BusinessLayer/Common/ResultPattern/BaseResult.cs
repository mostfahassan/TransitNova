using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.Domain.Enums.Result;
using System.Text.Json.Serialization;

namespace TransitNova.BusinessLayer.Common.ResultPattern
{
    public class BaseResult : IResult
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public ResultStatus Status { get; }
        public string? Message { get; init; }
        public int StatusCode => (int)Status;
        public Error? Error { get; }
        public IReadOnlyList<Error> Errors { get; }


        [JsonConstructor]
        public BaseResult(bool isSuccess, ResultStatus status, Error? error = null, IReadOnlyList<Error>? errors = null, string? message = null)
        {
            IsSuccess = isSuccess;
            Status = status;
            Message = message;
            Error = error;
            Errors = errors?.ToList() ?? [];
        }

        public static BaseResult Success()
            => new(true, ResultStatus.Success);
        public static BaseResult NoContent()
            => new(true, ResultStatus.NoContent);
        public static BaseResult Created(string? message = null)
            => new(true, ResultStatus.Created) { Message = message };

        public static BaseResult Failure(Error error)
            => new(false, ResultStatus.Failure, error);

        public static BaseResult Validation(IEnumerable<Error> errors)
        {
            var errorList = errors.ToList();
            return new(false, ResultStatus.ValidationError, errorList.FirstOrDefault(), errorList);
        }

        public static BaseResult NotFound(Error error)
            => new(false, ResultStatus.NotFound, error);

        public static BaseResult Unauthorized(Error error)
            => new(false, ResultStatus.Unauthorized, error);

        public static BaseResult Forbidden(Error error)
            => new(false, ResultStatus.Forbidden, error);

        public static BaseResult Conflict(Error error)
            => new(false, ResultStatus.Conflict, error);
        public static BaseResult UnExpected(Error error)
            => new(false, ResultStatus.UnExpected, error);
    }
}
