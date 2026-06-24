namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminOperationalHealth
    {
        Task<int> ActiveOperationManagersAsync(CancellationToken cancellationToken);

        Task<decimal> AverageCarrierRatingAsync(CancellationToken cancellationToken);

        Task<int> BusyCarriersAsync(CancellationToken cancellationToken);
        Task<int> AvailableCarriersAsync(CancellationToken cancellationToken);

        Task<int> ActiveShipmentAsync(CancellationToken cancellationToken);

        Task<decimal> CancelledShipmentRateAsync(CancellationToken cancellationToken);

        Task<decimal> DeliverySuccessRateAsync(CancellationToken cancellationToken);

        Task<int> GetActiveCarriersCountAsync(CancellationToken cancellationToken);
    }
}
