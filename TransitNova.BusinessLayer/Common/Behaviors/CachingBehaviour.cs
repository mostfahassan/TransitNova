
using MediatR;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public class CachingBehaviour<TRequest, TResponse>(ICacheService cacheService) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : BaseResult
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is  ICachable cachable)
            {
                var cachedResponse = await cacheService.GetAsync<TResponse>(cachable.CacheKey);
                if (cachedResponse is not null)
                    return cachedResponse;
            }
            var response = await next();

            if (request is ICachable cache)
            {
                await cacheService.SetAsync(
                    cache.CacheKey,
                    response,
                    CacheKeys.DefaultExpiration);
            }
            return response;
        }
    }
}
