using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.CarrierRepo
{
    internal class CarrierRulesValidation(AppDbContext context) : ICarrierRulesRepository
    {
        public Task<bool> CarrierExistsAsync(Guid carrierId, CancellationToken ct = default)
         => context.Carriers.AnyAsync(c => c.Id == carrierId || c.AppUserId == carrierId, ct);
        public async Task<bool> IsCarrierOwnerAsync(Guid carrierId, Guid userId, CancellationToken ct = default)
         => await context.Carriers.AnyAsync(c => c.AppUserId == carrierId && c.AppUserId == userId, ct);
        public async Task<bool> IsCarrierAvailableForAssignmentAsync(Guid carrierId, CancellationToken ct = default)
         => await context.Carriers.AnyAsync(c => c.Id == carrierId && c.Status == CarrierStatus.Available, ct);
        public Task<bool> IsCarrierInStatusAsync(Guid carrierId, CarrierStatus status, CancellationToken ct = default)
         => context.Carriers.AnyAsync(c => c.Id == carrierId && c.Status == status, ct);
        public Task<bool> IsCarrierHasCompletedProfileAsync(Guid appUserId, CancellationToken ct = default)
         => context.Carriers.AnyAsync(c => c.AppUserId == appUserId && c.HasAdditionalInfo ,ct);
    }
}
