using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository
{
    public interface ICarrierRulesRepository
    {
        Task<bool> IsCarrierAvailableForAssignmentAsync(Guid carrierId, CancellationToken ct = default);
        Task<bool> CarrierExistsAsync(Guid carrierId, CancellationToken ct = default);
        Task<bool> IsCarrierOwnerAsync(Guid carrierId, Guid userId, CancellationToken ct = default);
        Task<bool> IsCarrierInStatusAsync(Guid carrierId, CarrierStatus status, CancellationToken ct = default);
        Task<bool> IsCarrierHasCompletedProfileAsync(Guid appUserId, CancellationToken ct = default);
    }
}
