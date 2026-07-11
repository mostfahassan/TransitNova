using MediatR;
using TransitNovaPayment.Busieness.Common.Contracts.Keys;
using TransitNovaPayment.Busieness.Interfaces.Common;

namespace TransitNovaPayment.Busieness.Common.Behaviour
{
    public sealed class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is ICachable cachable)
            {
                var cachedResponse = await cacheService.GetAsync<TResponse>(cachable.CacheKey, cancellationToken);
                if (cachedResponse is not null)
                    return cachedResponse;
            }

            var response = await next(cancellationToken);

            if (request is ICachable cache)
            {
                await cacheService.SetAsync(
                    cache.CacheKey,
                    response,
                    CacheKeys.DefaultExpiration,
                    cancellationToken);
            }

            return response;
        }
    }
}