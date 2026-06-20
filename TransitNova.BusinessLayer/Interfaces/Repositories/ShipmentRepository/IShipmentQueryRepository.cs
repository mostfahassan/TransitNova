
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository
{
    public interface IShipmentQueryRepository
    {
        Task<Shipment?> GetEntityAsync(Guid shipmentId, CancellationToken ct);
        Task<RetrieveShipmentDto?> CreateShipmentForUserAsync(Guid shipmentId, CancellationToken ct);
        Task<RetrieveShipmentSummaryDto?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct);
        Task<Shipment?> GetShipmentInStatusAsync(Guid shipmentId, ShipmentStatuses shipmentStatus, CancellationToken ct, bool includes = false);
        Task<IEnumerable<Shipment>> GetShipmentsAssignedToCarrierAsync(ShipmentStatuses shipmentStatus, Guid carrierId, CancellationToken ct);
        Task<IEnumerable<RetrieveShipmentStatusDto>> GetShipmentHistoriesAsync(Guid shipmentId, CancellationToken cancellationToken);
        Task<PagedResult<RetrieveShipmentDto>> FilterAsync(ShipmentFilterDto filter, CancellationToken ct);
        Task<Shipment?> GetShipmentForCommands(Guid shipmentId, CancellationToken ct);
        Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatus(CancellationToken cancellationToken);
    }
}
