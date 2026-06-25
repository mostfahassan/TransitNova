using System.Collections.Concurrent;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;

namespace TransitNovaPayment.Busieness.Common.Implementation
{
    internal sealed class MemoryCacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

        public Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken)
            where TResponse : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_cache.TryGetValue(key, out var entry))
            {
                return Task.FromResult<TResponse?>(null);
            }

            if (entry.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult<TResponse?>(null);
            }

            return Task.FromResult(entry.Response as TResponse);
        }

        public Task SetAsync<TResponse>(
            string key,
            TResponse response,
            TimeSpan expiration,
            CancellationToken cancellationToken)
            where TResponse : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            _cache[key] = new CacheEntry(response, DateTimeOffset.UtcNow.Add(expiration));
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var key in _cache.Keys.Where(key => key.StartsWith(prefix, StringComparison.Ordinal)).ToArray())
            {
                _cache.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }

        private sealed record CacheEntry(object Response, DateTimeOffset ExpiresAt);
    }
}