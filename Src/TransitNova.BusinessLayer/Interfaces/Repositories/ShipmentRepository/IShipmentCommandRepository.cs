using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository
{
    public interface IShipmentCommandRepository
    {
        Task AddAsync(Shipment shipment, CancellationToken ct);
       
        Task UpdateAsync(Shipment shipment, CancellationToken ct);
     
    }
}
