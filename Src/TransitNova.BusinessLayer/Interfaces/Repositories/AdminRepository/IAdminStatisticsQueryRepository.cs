using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminStatisticsQueryRepository
    {
     
        Task<int> GetTotalCarriersAsync(CancellationToken cancellationToken);

        Task<int> GetTotalOperationManagersCountAsync(CancellationToken cancellationToken);

        Task<int> GetTotalShipmentsCountAsync(CancellationToken cancellationToken);

        Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken);

        Task<int> GetTotalActiveTripsAsync(CancellationToken cancellationToken);
    }
}
