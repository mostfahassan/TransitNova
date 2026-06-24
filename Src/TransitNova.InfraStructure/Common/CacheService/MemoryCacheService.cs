
using Microsoft.Extensions.Caching.Memory;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.InfraStructure.Common.CacheService
{
    internal class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        public Task<T?> GetAsync<T>(string key,CancellationToken cancellationToken)
        {
            cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        {
            var size = key switch
            {
                string cacheKey when cacheKey == CacheKeys.BundleList() => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:trips:carrier-id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CitiesPrefix}:government-id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:shipment-histories:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:dashboard:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:shipments:carrier-id:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CitiesPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey == CacheKeys.OperationManagerDashboard() => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:handled-carriers:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:handled-shipments:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:dashboard:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.ZonesPrefix}:filter:", StringComparison.Ordinal) => 10,
                _ => 1
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                Size = size
            };

            cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
