using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminActivityQueryRepository
    {
        Task<List<AdminActivityDto>> GetRecentActivitiesAsync(CancellationToken cancellationToken, int count = 10);

        Task<List<RetrieveShipmentDto>> GetRecentShipmentsAsync(CancellationToken cancellationToken, int count = 10);

        Task<List<TopCarrierDto>> GetTopCarriersAsync(CancellationToken cancellationToken, int count = 10);

        Task<List<TopOperationManagerDto>> GetTopOperationManagersAsync(CancellationToken cancellationToken, int count = 10);

        Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(CancellationToken cancellationToken);
        Task<RevenueSummaryDto> GetRevenueSummaryAsync(CancellationToken cancellationToken);
    }
}
