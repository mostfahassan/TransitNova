

using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.User
{
    internal class UserRulesRepository(AppDbContext context) : IUserRulesRepository
    {
        public async Task<bool> OwnsAccountAsync(Guid AppUserId, CancellationToken cancellationToken)

             => await context.UserProfiles.AnyAsync(up => up.AppUserId == AppUserId, cancellationToken);
        

        public async Task<bool> OwnsShipmentAsync(Guid AppUserId, Guid ShipmentId, CancellationToken ct)
            => await context.UserProfiles.AnyAsync(up => up.AppUserId == AppUserId && up.SentShipments.Any(s => s.Id == ShipmentId), ct);
    }
}
