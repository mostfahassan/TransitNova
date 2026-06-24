using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.CarrierRepo;

public class CarrierCommandRepository(AppDbContext context) : ICarrierCommandRepository
{
    public async Task<int> DeleteCarrierAsync(Guid carrierId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await context.Carriers
            .Where(c => c.Id == carrierId && !c.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.IsDeleted, true)
                .SetProperty(p => p.Status, CarrierStatus.InActive) 
                .SetProperty(p => p.DeletedOn, now), ct);
    }

    public async Task<int> UpdateStatusAsync(Guid userId, CarrierStatus newStatus, CancellationToken ct = default)
         => await context.Carriers
            .Where(c => c.AppUserId == userId)
            .ExecuteUpdateAsync(p => p.SetProperty(p => p.Status, newStatus), ct);
   

}