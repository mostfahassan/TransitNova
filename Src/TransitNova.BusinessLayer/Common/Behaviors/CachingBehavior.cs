
using MediatR;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResult
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is  ICachable cachable)
            {
                var cachedResponse = await cacheService.GetAsync<TResponse>(cachable.CacheKey,cancellationToken);
                if (cachedResponse is not null)
                    return cachedResponse;
            }
            var response = await next(cancellationToken);
            if (response.IsSuccess)
            {
                if (request is ICachable cache)
                {
                    await cacheService.SetAsync(cache.CacheKey, response, CacheKeys.DefaultExpiration, cancellationToken);
                }
            }
            return response;
        }
    }
}
