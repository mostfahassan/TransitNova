
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository
{
    public interface ICarrierShipmentQueryRepository
    {
        Task<RetrieveShipmentDto?> GetCarrierShipmentAsync(Guid carrierId, Guid shipmentId, CancellationToken ct = default);

        Task<IEnumerable<RetrieveShipmentDto>> GetCarrierShipmentsAsync(Guid carrierId, CancellationToken ct = default);

        Task<PagedResult<RetrieveShipmentDto>> GetCarrierShipmentsAsync(Guid carrierId, CarrierShipmentFilterDto filter, CancellationToken ct = default);

        Task<Dictionary<ShipmentStatuses, int>> GetCarrierShipmentCountInStatusAsync(Guid carrierId, CancellationToken ct = default);
    }
}
