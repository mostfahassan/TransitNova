using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRatingRepo;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.CarrierRatings
{
    internal class CarrierRatingCommandsRepository(AppDbContext context) : ICarrierRatingCommandsRepository
    {
        public async Task AddRatingAsync(CarrierRating rating, CancellationToken cancellationToken)
          => await context.CarrierRatings.AddAsync(rating, cancellationToken);
    }
}
