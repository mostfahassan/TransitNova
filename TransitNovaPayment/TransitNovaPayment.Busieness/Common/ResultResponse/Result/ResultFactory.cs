using System.Reflection;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;

namespace TransitNovaPayment.Busieness.Common.ResultResponse.Result
{
    public static class ResultFactory
    {
        public static TResponse Validation<TResponse>(IEnumerable<Error> errors) where TResponse : IResult
        {
            var responseType = typeof(TResponse);

            var method = responseType.GetMethod(nameof(BaseResult.Validation), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                ?? throw new InvalidOperationException($"Validation factory not found on {responseType.Name}"); ;

            return (TResponse)method.Invoke(null, [errors])!;
        }
    }
}
