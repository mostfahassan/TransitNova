using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
namespace TransitNovaPayment.Infrastructure.Service.Caching
{
    internal class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        private readonly ConcurrentDictionary<string, byte> _keys = new();

        public Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken)
            where TResponse : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            cache.TryGetValue(key, out TResponse? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<TResponse>(string key, TResponse value, TimeSpan expiration, CancellationToken cancellationToken)
            where TResponse : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                Size = 100
            }.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
            {
                if (evictedKey is string cacheKey)
                {
                    _keys.TryRemove(cacheKey, out _);
                }
            });

            cache.Set(key, value, options);
            _keys.TryAdd(key, 0);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var key in _keys.Keys.Where(key => key.StartsWith(prefix, StringComparison.Ordinal)).ToArray())
            {
                cache.Remove(key);
                _keys.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }
    }
}