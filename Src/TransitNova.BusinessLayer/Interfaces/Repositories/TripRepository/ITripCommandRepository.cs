using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository
{
    public interface ITripCommandRepository
    {
        Task StartNewTripAsync(Trip trip, CancellationToken cancellationToken);
        Task<Trip?> GetTripForCommandsAsync(Guid tripId, CancellationToken cancellationToken);
        
    }
}
