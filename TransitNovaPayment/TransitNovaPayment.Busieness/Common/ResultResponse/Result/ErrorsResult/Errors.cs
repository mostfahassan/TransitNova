using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult.Enum;
namespace TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult
{
    public static class Errors
    {
        public static Error Failure (string message)
            => new(ErrorCode.FAILED, message);
        public static Error Validation(string message)
            => new(ErrorCode.VALIDATION_ERROR, message);
    }
}
