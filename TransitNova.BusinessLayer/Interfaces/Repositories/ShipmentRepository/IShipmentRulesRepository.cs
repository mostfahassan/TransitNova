
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository
{
    public interface IShipmentRulesRepository
    {
        Task<bool> Editable(Guid shipmentId, ShipmentStatuses[] status, CancellationToken ct);
        Task<bool> Exists(Guid shipmentId, CancellationToken ct);
        Task<bool> CanRateDeliveryCarrierAsync(Guid shipmentId,Guid carrierId,Guid senderId, CancellationToken ct);
        Task<bool> CanRatePickUpCarrierAsync(Guid shipmentId,Guid carrierId,Guid senderId, CancellationToken ct);
    }
}
