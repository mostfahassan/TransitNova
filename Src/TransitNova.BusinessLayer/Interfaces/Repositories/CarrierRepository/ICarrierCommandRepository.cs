using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository
{
    public interface ICarrierCommandRepository
    {
        Task<int> UpdateStatusAsync(Guid userId, CarrierStatus status, CancellationToken ct = default);

        Task<int> DeleteCarrierAsync(Guid carrierId, CancellationToken ct = default);
    }
}
