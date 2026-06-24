
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository
{
    public interface IShipmentRulesRepository
    {
        Task<bool> IsEditableAsync(Guid shipmentId, ShipmentStatuses[] status, CancellationToken ct);
        Task<bool> ExistsAsync(Guid shipmentId, CancellationToken ct);
        Task<bool> CanRateDeliveryCarrierAsync(Guid shipmentId,Guid carrierId,Guid senderId, CancellationToken ct);
        Task<bool> CanRatePickupCarrierAsync(Guid shipmentId,Guid carrierId,Guid senderId, CancellationToken ct);
    }
}
