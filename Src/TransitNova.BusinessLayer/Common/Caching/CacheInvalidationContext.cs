using System.Runtime.CompilerServices;

namespace TransitNova.BusinessLayer.Common.Caching
{
    public static class CacheInvalidationContext
    {
        private sealed class CacheKeyBag
        {
            public List<string> Keys { get; } = [];
        }

        private static readonly ConditionalWeakTable<ICacheInvalidator, CacheKeyBag> RegisteredKeys = new();

        public static void Set(ICacheInvalidator invalidator, params string[] cacheKeys)
        {
            var bag = RegisteredKeys.GetOrCreateValue(invalidator);
            bag.Keys.Clear();
            AddCore(bag, cacheKeys);
        }

        public static void Add(ICacheInvalidator invalidator, params string[] cacheKeys)
        {
            var bag = RegisteredKeys.GetOrCreateValue(invalidator);
            AddCore(bag, cacheKeys);
        }

        internal static IReadOnlyList<string> GetRegisteredKeys(ICacheInvalidator invalidator)
            => RegisteredKeys.TryGetValue(invalidator, out var bag)
                ? bag.Keys
                : Array.Empty<string>();

        private static void AddCore(CacheKeyBag bag, IEnumerable<string> cacheKeys)
        {
            foreach (var cacheKey in cacheKeys)
            {
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    bag.Keys.Add(cacheKey);
                }
            }
        }
    }
}
