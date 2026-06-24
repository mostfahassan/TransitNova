
using System.Reflection;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public static class ResultFactory
    {
        public static TResponse Validation<TResponse>(IEnumerable<Error> errors) where TResponse : IResult
        {
            var responseType = typeof(TResponse);

            var method = responseType.GetMethod(nameof(BaseResult.Validation), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (method is null)
            {
                throw new InvalidOperationException(
                    $"Validation factory not found on {responseType.Name}");
            }
            return (TResponse)method.Invoke(null, [errors])!;
        }
    }
}
