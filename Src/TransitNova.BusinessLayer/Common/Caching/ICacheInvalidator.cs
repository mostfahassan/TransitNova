namespace TransitNova.BusinessLayer.Common.Caching
{
    public interface ICacheInvalidator
    {
        IReadOnlyList<string> CacheKeysToInvalidate => CacheInvalidationContext.GetRegisteredKeys(this);
    }
}
