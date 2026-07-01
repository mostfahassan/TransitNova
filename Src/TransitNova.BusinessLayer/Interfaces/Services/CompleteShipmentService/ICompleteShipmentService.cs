
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Services.CompleteShipmentService
{
    public interface ICompleteShipmentService
    {
        Task<Shipment> CompleteShipmentToWarehouseAsync(Guid ShipmentId, Guid CarrierId, CancellationToken cancellationToken);
        Task<Shipment> CompleteShipmentDeliveryAsync(Guid ShipmentId, Guid CarrierId, CancellationToken cancellationToken);

    }
}
