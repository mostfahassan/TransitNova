
namespace TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository
{
    public interface IUserRulesRepository
    {
        public Task<bool> OwnsShipmentAsync(Guid AppUserId, Guid ShipmentId, CancellationToken ct);
        public Task<bool> OwnsAccountAsync(Guid AppUserId, CancellationToken cancellationToken);
    }
}
