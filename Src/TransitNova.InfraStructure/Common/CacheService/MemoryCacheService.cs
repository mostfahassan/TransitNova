
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
                string cacheKey when cacheKey == CacheKeys.Admins.Dashboard => 10,
                string cacheKey when cacheKey == CacheKeys.Bundles.List => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.BundlesPrefix}:subscription-id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:profile:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:shipment-details:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:trips:carrier-id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:trip-details:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CitiesPrefix}:id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CitiesPrefix}:government-id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:shipment-histories:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:dashboard:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CarriersPrefix}:shipments:carrier-id:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.CitiesPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey == CacheKeys.OperationManagers.Dashboard => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:handled-carriers:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.OperationManagersPrefix}:handled-shipments:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.ShipmentsPrefix}:tracking-number:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.TripsPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.TripsPrefix}:id:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:admin-details:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:dashboard:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:filter:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:profile:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.UsersPrefix}:shipment:", StringComparison.Ordinal) => 5,
                string cacheKey when cacheKey == CacheKeys.Vehicles.List => 5,
                string cacheKey when cacheKey.StartsWith("warehouse-managers:dashboard:manager-id:", StringComparison.Ordinal) => 10,
                string cacheKey when cacheKey.StartsWith($"{CacheKeys.ZonesPrefix}:filter:", StringComparison.Ordinal) => 10,
                _ => 5
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

