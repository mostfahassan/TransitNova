
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository
{
    public interface ICarrierDashboardRepository
    {
        Task<PagedResult<RetrieveShipmentDto>> GetCarrierShipmentsAsync(Guid carrierId, CarrierShipmentFilterDto filter, CancellationToken ct = default);
        Task<Dictionary<ShipmentStatuses, int>> GetCarrierShipmentCountInStatusAsync(Guid carrierId, CancellationToken ct = default);
        Task<List<CarrierTripDto>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken);
        Task<decimal> GetCarrierRevenueAsync(Guid carrierId, CancellationToken ct = default);
    }
}
