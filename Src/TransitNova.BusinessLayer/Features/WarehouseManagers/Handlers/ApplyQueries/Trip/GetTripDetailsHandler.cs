using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.Trips.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;

namespace TransitNova.BusinessLayer.Features.Trips.Handlers
{
    public sealed class GetTripDetailsHandler(ITripQueryRepository tripQuery, ILogger<GetTripDetailsHandler> logger)
        : IQueryHandler<GetTripDetailsQuery, Result<TripDetailsDto>>
    {
        public async Task<Result<TripDetailsDto>> Handle(GetTripDetailsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetTripDetailsQuery for TripId: {TripId} and HandlerId: {HandlerId}", request.TripId, request.HandlerId);

            var trip = request.HandlerId.HasValue
                ? await tripQuery.GetTripAsync(t => t.Id == request.TripId && t.Carrier.HandlerId == request.HandlerId.Value, cancellationToken)
                : await tripQuery.GetTripAsync(t => t.Id == request.TripId, cancellationToken);

            if (trip is null)
            {
                logger.LogWarning("Trip with ID {TripId} and HandlerId {HandlerId} was not found.", request.TripId, request.HandlerId);
                return Result<TripDetailsDto>.NotFound(Errors.TripNotFound($"Trip With Id => {request.TripId} Not Found"));
            }

            logger.LogInformation("Successfully retrieved details for Trip with ID {TripId}.", request.TripId);
            return Result<TripDetailsDto>.Success(trip);
        }
    }
}