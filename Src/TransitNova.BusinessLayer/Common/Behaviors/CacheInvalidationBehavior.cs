using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
        ICacheService cache,
        ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);

            if (request is not ICacheInvalidator invalidator || !IsSuccess(response))
            {
                return response;
            }

            var keys = invalidator.CacheKeysToInvalidate
                .Concat(CacheInvalidationContext.GetRegisteredKeys(invalidator))
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (keys.Length == 0)
            {
                return response;
            }

            logger.LogDebug("[CacheInvalidation] {Command} invalidating {Count} key(s): {Keys}", typeof(TRequest).Name,keys.Length,string.Join(", ", keys));

            await Task.WhenAll(keys.Select(cache.RemoveAsync));
            return response;
        }

        private static bool IsSuccess(TResponse response)
           => response is not null && response.IsSuccess;
    }
}
