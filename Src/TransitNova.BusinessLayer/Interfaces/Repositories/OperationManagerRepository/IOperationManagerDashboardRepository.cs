using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository
{
    public interface IOperationManagerDashboardRepository
    {
        Task<(int TotalHandledShipments, int TotalHandledCarriers)> GetHandledSummaryAsync(Guid operationManagerId, CancellationToken cancellationToken);
        Task<PagedResult<RetrieveShipmentSummaryDto>> GetShipmentsAsync(CancellationToken ct, int pageNumber = 1, int pageSize = 20);
        Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(CancellationToken cancellationToken);
    }
}
