using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.Features.WarehouseManagers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
namespace TransitNova.BusinessLayer.Features.WarehouseManagers.Handlers.ApplyQueries.Trip
{
    public sealed class GetTripDetailsHandler(ITripQueryRepository tripQuery ,ILogger<GetTripDetailsHandler> logger) : IQueryHandler<GetTripDetailsQuery, Result<TripDetailsDto>>
    {
        public async Task<Result<TripDetailsDto>> Handle(GetTripDetailsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetTripDetailsQuery for TripId: {TripId}", request.TripId);

            var trip = await tripQuery.GetTripAsync(request.TripId, cancellationToken);

            if (trip is null)
            {
                logger.LogWarning("Trip with ID {TripId} was not found.", request.TripId);

                return Result<TripDetailsDto>.NotFound(Errors.TripNotFound($"Trip With Id => {request.TripId} Not Found"));
            }
            logger.LogInformation("Successfully retrieved details for Trip with ID {TripId}.", request.TripId);

            return Result<TripDetailsDto>.Success(trip);
        }
    }
}
