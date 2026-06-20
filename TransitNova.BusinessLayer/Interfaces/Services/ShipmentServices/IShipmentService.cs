
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices
{
    public interface IShipmentService
    {
        void UpdateShipmentDetails(Shipment shipment, UpdateShipmentDto shipmentCommand);
        Task<Guid> PrepareShipmentCreation(CreateShipmentDto Dto , Guid AppUserId,CancellationToken cancellationToken);
    }
}
