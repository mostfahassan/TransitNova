using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern.Enums;
using TransitNovaPayment.Busieness.DTO.PaymentDto;
namespace TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern
{
    public class BaseResult : IResult
    {
        public PaymentDetailsDto? Data { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public ResultStatus Status { get; }
        public string? Message { get; init; }
        public int StatusCode => (int)Status;
        public Error? Error { get; }
  
        private BaseResult(bool succeded, ResultStatus result, Error? error = null, string? message = null, PaymentDetailsDto? data = null)
        {
            IsSuccess = succeded;
            Status = result;
            Error = error;
            Message = message;
            Data = data;
        }

        public static BaseResult Success(PaymentDetailsDto data)
        {
            return new BaseResult(true, ResultStatus.Success, null, "PaymentProcess completed successfully.", data);
        }

        public static BaseResult Failure(Error error)
        {
            return new BaseResult(false, ResultStatus.BadRequest, error, "PaymentProcess processing failed.", null);
        }

        public static BaseResult Unauthorized(Error error)
        {
            return new BaseResult(false, ResultStatus.Unauthorized, error, "PaymentProcess authentication failed.", null);
        }

        public static BaseResult Validation(IEnumerable<Error> errors)
        {
            var errorList = string.Join(",", errors.ToList());
            return new(false, ResultStatus.ValidationError, null, errorList);
        }
    }
}
