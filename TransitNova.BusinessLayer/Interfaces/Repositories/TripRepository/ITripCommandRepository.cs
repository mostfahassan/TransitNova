using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository
{
    public interface ITripCommandRepository
    {
        Task StartNewTrip(Trip trip, CancellationToken cancellationToken);
    }
}
