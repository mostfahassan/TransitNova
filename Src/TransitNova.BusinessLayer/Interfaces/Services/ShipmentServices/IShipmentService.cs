
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices
{
    public interface IShipmentService
    {
        void UpdateShipmentDetails(Shipment shipment, UpdateShipmentDto shipmentCommand);
        Task<(Result<InvoiceDto>, string)> HandleShipmentCreation(CreateShipmentDto Dto , Guid AppUserId,CancellationToken cancellationToken);
    }
}
