using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository
{
    public interface ITripQueryRepository
    {
        Task<List<CarrierTripDto>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken);
        Task<CarrierTripDto?> GetCarrierTripAsync(Guid carrierId, Guid tripId, CancellationToken cancellationToken);
        Task<TripDetailsDto?> GetTripAsync(Guid tripId, CancellationToken cancellationToken);
        Task<PagedResult<TripDetailsDto>> FilterTripsAsync(FilterTripsDto filterDto, CancellationToken cancellationToken);
     
    }
}
