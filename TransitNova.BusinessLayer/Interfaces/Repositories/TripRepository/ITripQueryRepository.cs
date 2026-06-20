using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository
{
    public interface ITripQueryRepository
    {
        Task<List<CarrierTripDto>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken);
        Task<CarrierTripDto?> GetCarrierTripAsync(Guid carrierId, Guid tripId, CancellationToken cancellationToken);
        Task<Trip?> GetTripByIdAsync(Guid tripId, CancellationToken cancellationToken);
        Task<TripDetailsDto?> GetTripAsync(Guid tripId, CancellationToken cancellationToken);
        Task< IEnumerable<TripDetailsDto>> GetTripsAsync(CancellationToken cancellationToken);
        Task< IEnumerable<TripDetailsDto>> GetTripByTypeAsync(TripType tripType, CancellationToken cancellationToken);
        Task< IEnumerable<TripDetailsDto>> GetTripByStatusAsync(TripStatus tripStatus , CancellationToken cancellationToken);
        Task<TripDetailsDto?> GetTripByTypeAsync(Guid tripId , TripType tripType, CancellationToken cancellationToken);
        Task<TripDetailsDto?> GetTripByStatusAsync(Guid tripId , TripStatus tripStatus ,CancellationToken cancellationToken);

    }
}
