using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierTripsHandler(
        ITripQueryRepository tripRepository,
        ILogger<GetCarrierTripsHandler> logger)
        : IQueryHandler<GetCarrierTripsQuery, Result<IReadOnlyCollection<CarrierTripDto>>>
    {
        public async Task<Result<IReadOnlyCollection<CarrierTripDto>>> Handle(GetCarrierTripsQuery request, CancellationToken ct)
        {
            logger.LogInformation("Handling GetCarrierTripsQuery for CarrierId: {CarrierId}", request.CarrierId);

            var trips = await tripRepository.GetCarrierTripsAsync(request.CarrierId, ct);
            var result = trips.Count == 0
                ? Result<IReadOnlyCollection<CarrierTripDto>>.Success([])
                : Result<IReadOnlyCollection<CarrierTripDto>>.Success(trips);
            return result;
        }
    }
}
