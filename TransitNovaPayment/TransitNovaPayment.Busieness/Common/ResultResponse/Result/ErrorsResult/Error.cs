using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult.Enum;

namespace TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult
{
    public sealed record Error(ErrorCode Code, string Message);
}
