
using System.Linq.Expressions;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository
{
    public interface IUserRulesRepository
    {
        public Task<bool> OwnsShipmentAsync(Guid AppUserId, Guid ShipmentId, CancellationToken ct);
    }
}
