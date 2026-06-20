
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRatingRepo
{
    public interface ICarrierRatingCommandsRepository
    {
        public Task AddRatingAsync(CarrierRating rating, CancellationToken cancellationToken);
    }
}
