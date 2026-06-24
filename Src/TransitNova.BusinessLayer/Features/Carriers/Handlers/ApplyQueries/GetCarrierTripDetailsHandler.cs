using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Trips;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierTripDetailsHandler(
        ITripQueryRepository tripRepository,
        ILogger<GetCarrierTripDetailsHandler> logger)
        : IQueryHandler<GetCarrierTripDetailsQuery, Result<CarrierTripDto>>
    {
        public async Task<Result<CarrierTripDto>> Handle(GetCarrierTripDetailsQuery request, CancellationToken ct)
        {
            var trip = await tripRepository.GetCarrierTripAsync(request.CarrierId, request.TripId, ct);
            if (trip is null)
            {
                logger.LogWarning("Carrier {UserId} attempted to open Trip {TripId}", request.CarrierId, request.TripId);
                var notFoundResult = Result<CarrierTripDto>.Failure(Errors.TripNotFound("Trip not found for this carrier."));
                return notFoundResult;
            }   

            var result = Result<CarrierTripDto>.Success(trip);
            return result;
        }
    }
}
