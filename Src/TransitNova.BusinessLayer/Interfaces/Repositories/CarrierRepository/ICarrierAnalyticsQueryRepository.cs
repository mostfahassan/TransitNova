namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository
{
    public interface ICarrierAnalyticsQueryRepository
    {
        Task<decimal?> GetCarrierRevenueAsync(Guid carrierId, CancellationToken ct = default);

        Task<decimal?> GetAverageRatingAsync(Guid carrierId, CancellationToken ct = default);
    }
}
