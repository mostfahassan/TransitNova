using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminDashboardBuilder
    {
        Task<List<AdminActivityDto>> GetRecentActivitiesAsync(CancellationToken cancellationToken, int count = 10);
        Task<List<RetrieveShipmentDto>> GetRecentShipmentsAsync(CancellationToken cancellationToken, int count = 10);
        Task<List<TopCarrierDto>> GetTopCarriersAsync(CancellationToken cancellationToken, int count = 10);
        Task<List<TopOperationManagerDto>> GetTopOperationManagersAsync(CancellationToken cancellationToken, int count = 10);
        Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(CancellationToken cancellationToken);
        Task<RevenueSummaryDto> GetRevenueSummaryAsync(CancellationToken cancellationToken);
        Task<UserStatistics> UsersStatsAsync(CancellationToken cancellationToken);
        Task<TripsStatistics> TripsStatsAsync(CancellationToken cancellationToken);
        Task<OperationManagerStats> OperationManagerStatsAsync(CancellationToken cancellationToken);
        Task<ShipmentStats> ShipmentsStatsAsync(CancellationToken cancellationToken);
        Task<CarrierStats> GetCarriersActivityStatsAsync(CancellationToken cancellationToken);
    }
}
