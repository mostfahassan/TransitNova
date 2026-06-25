using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
namespace TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern
{
    public interface IResult
    {
        Error? Error { get; }
        bool IsSuccess { get; }
    }
}
