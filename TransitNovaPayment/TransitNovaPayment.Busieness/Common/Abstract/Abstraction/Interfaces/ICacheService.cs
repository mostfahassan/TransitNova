namespace TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces
{
    public interface ICacheService
    {
        Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken)
            where TResponse : class;

        Task SetAsync<TResponse>(
            string key,
            TResponse response,
            TimeSpan expiration,
            CancellationToken cancellationToken)
            where TResponse : class;

        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken);
    }
}