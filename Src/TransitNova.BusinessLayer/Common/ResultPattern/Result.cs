using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.Domain.Enums.Result;
using System.Text.Json.Serialization;

namespace TransitNova.BusinessLayer.Common.ResultPattern
{
    public class Result<T> : BaseResult
    {
        [JsonConstructor]
        public Result(T? data, bool isSuccess, ResultStatus status, Error? error = null, IReadOnlyList<Error>? errors = null, string? message = null)
        : base(isSuccess, status, error, errors, message)
        {
            Data = data;
        }
        public T? Data { get; }
        public static Result<T> Success(T data)
            => new(data, true, ResultStatus.Success);


        public new static Result<T> Failure(Error error)
            => new(default, false, ResultStatus.Failure, error);
        public new static Result<T> Validation(IEnumerable<Error> errors)
        {
            var errorList = errors.ToList();
            return new(default, false, ResultStatus.ValidationError, errorList.FirstOrDefault(), errorList);
        }
        public static Result<T> Created(T data)
            => new(data, true, ResultStatus.Created);

        public new static Result<T> NotFound(Error error)
            => new(default, false, ResultStatus.NotFound, error);

        public new static Result<T> Unauthorized(Error error)
            => new(default, false, ResultStatus.Unauthorized, error);

        public new static Result<T> Forbidden(Error error)
            => new(default, false, ResultStatus.Forbidden, error);

        public new static Result<T> Conflict(Error error)
            => new(default, false, ResultStatus.Conflict, error);
    }
}
